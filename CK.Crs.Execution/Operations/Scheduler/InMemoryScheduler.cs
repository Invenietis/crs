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

        CancellationTokenSource _source;
        readonly IServiceProvider _applicationServices;
        readonly ICrsConfiguration _configuration;

        public InMemoryScheduler( ICrsConfiguration configuration, IServiceProvider services )
        {
            if( configuration == null ) throw new ArgumentNullException( nameof( configuration ) );
            if( services == null ) throw new ArgumentNullException( nameof( services ) );

            _applicationServices = services;
            _configuration = configuration;

            _timers = new List<Timer>();
            _source = new CancellationTokenSource();
        }

        public void Dispose()
        {
            foreach( var t in _timers ) t.Dispose();
        }

        public async void TimerCallback( object state ) => await ExecuteOperation( state as ScheduledCommand );

        private async Task ExecuteOperation( ScheduledCommand operation )
        {
            using( var pipeline = new CommandSchedulingPipeline( _applicationServices, _configuration, operation ) )
            {
                // Provides specific steps for the command scheduling :
                // - ICommandFilter implementation will be different: they will not rely on HttpContext for example.
                // - AmbientValue validator will also be different.
                // OR : schedule a command is : 
                // - Handle the command A
                // - Schedule the command B. But before doing the real scheduling, apply CommandFilters, AmbientValueValidation etc, in the command A pipeline.
                // Ot : schedule a command is full trust on the command scheduling, since the command is crafted server-side.
                //await new CommandFiltersInvoker( _factory ).Invoke( pipeline, _source.Token );

                var h =  pipeline.CommandServices.GetService<IExecutionFactory>();
                var r =  pipeline.CommandServices.GetService<ICommandRegistry>();
                await new SyncCommandExecutor( h, r ).TryInvoke( pipeline, pipeline.CancellationToken );
                pipeline.Monitor.Info().Send( "ScheduledCommand executed {0}.", operation.CommandId );
            }
        }

        public void Execute( IActivityMonitor monitor, ScheduledCommand operation )
        {
            if( operation.Scheduling.WhenCommandShouldBeRaised <= DateTime.UtcNow )
            {
                Task.Factory.StartNew( TimerCallback, operation ).Wait();
            }
            else
            {
                Timer t = new Timer( TimerCallback, operation, (int) (operation.Scheduling.WhenCommandShouldBeRaised - DateTime.UtcNow ).TotalMilliseconds, Timeout.Infinite);
                _timers.Add( t );
            }
        }
    }
}
