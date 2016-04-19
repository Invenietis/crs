using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{
    public class InMemoryScheduler : IOperationExecutor<ScheduledCommand>, IDisposable
    {
        readonly IList<Timer> _timers;
        readonly ICrsConfiguration _config;
        readonly SyncCommandExecutor _executor;
        readonly IServiceProvider _serviceProvider;

        CancellationTokenSource _source;

        public InMemoryScheduler( IServiceProvider serviceProvider, ICrsConfiguration config, ICommandExecutionFactories factories )
        {
            if( serviceProvider == null ) throw new ArgumentNullException( nameof( serviceProvider ) );
            if( config == null ) throw new ArgumentNullException( nameof( config ) );
            if( factories == null ) throw new ArgumentNullException( nameof( factories ) );

            _serviceProvider = serviceProvider;
            _config = config;
            _executor = new SyncCommandExecutor( factories );

            _timers = new List<Timer>();
            _source = new CancellationTokenSource();
        }

        public void Dispose()
        {
            foreach( var t in _timers ) t.Dispose();
        }

        public async void TimerCallback( object state )
        {
            var scheduledOperation = state as ScheduledCommand;

            // TODO: provides access to the scheduling pipeline configuration
            using( var pipeline = new CommandSchedulingPipeline( _serviceProvider, _config, scheduledOperation ) )
            {
                // Provides specific steps for the command scheduling :
                // - ICommandFilter implementation will be different: they will not rely on HttpContext for example.
                // - AmbientValue validator will also be different.
                // OR : schedule a command is : 
                // - Handle the command A
                // - Schedule the command B. But before doing the real scheduling, apply CommandFilters, AmbientValueValidation etc, in the command A pipeline.
                // Ot : schedule a command is full trust on the command scheduling, since the command is crafted server-side.
                //await new CommandFiltersInvoker( _factory ).Invoke( pipeline, _source.Token );

                await _executor.Invoke( pipeline, _source.Token );
            }
        }

        public void Execute( IActivityMonitor monitor, ScheduledCommand operation )
        {
            Timer t = new Timer( TimerCallback, operation, (int) (operation.Scheduling.WhenCommandShouldBeRaised - DateTime.UtcNow ).TotalMilliseconds, Timeout.Infinite);
            _timers.Add( t );
        }
    }
}
