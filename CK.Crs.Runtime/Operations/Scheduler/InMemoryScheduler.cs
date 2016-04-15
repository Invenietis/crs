using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime.Pipeline;

namespace CK.Crs.Runtime
{
    public class InMemoryScheduler : IOperationExecutor<ScheduledCommand>, IDisposable
    {
        readonly IList<Timer> _timers;

        readonly IPipelineConfiguration _config;
        readonly ICommandRouteCollection _routes;
        readonly IExecutionStrategySelector _executorSelector;
        readonly IFactories _factory;

        CancellationTokenSource _source;
        IServiceProvider _serviceProvider;
        public InMemoryScheduler( IServiceProvider serviceProvider, IPipelineConfiguration config, IExecutionStrategySelector executor, ICommandRouteCollection routes, IFactories factory )
        {
            if( config == null ) throw new ArgumentNullException( nameof( config ) );
            if( executor == null ) throw new ArgumentNullException( nameof( executor ) );
            if( factory == null ) throw new ArgumentNullException( nameof( factory ) );
            if( routes == null ) throw new ArgumentNullException( nameof( routes ) );

            _serviceProvider = serviceProvider;
            _config = config;
            _executorSelector = executor;
            _factory = factory;
            _routes = routes;

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
                await new CommandFiltersInvoker( _factory ).Invoke( pipeline, _source.Token );
                await new CommandExecutor( _factory, _executorSelector ).Invoke( pipeline, _source.Token );
            }
        }

        public void Execute( IActivityMonitor monitor, ScheduledCommand operation )
        {
            Timer t = new Timer( TimerCallback, operation, (int) (operation.Scheduling.WhenCommandShouldBeRaised - DateTime.UtcNow ).TotalMilliseconds, Timeout.Infinite);
            _timers.Add( t );
        }
    }
}
