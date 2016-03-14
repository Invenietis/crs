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

        readonly PipelineEvents _events;
        readonly ICommandRouteCollection _routes;
        readonly IExecutionStrategySelector _executorSelector;
        readonly IFactories _factory;

        CancellationTokenSource _source;

        public InMemoryScheduler( PipelineEvents events, IExecutionStrategySelector executor, ICommandRouteCollection routes, IFactories factory )
        {
            if( events == null ) throw new ArgumentNullException( nameof( events ) );
            if( executor == null ) throw new ArgumentNullException( nameof( executor ) );
            if( factory == null ) throw new ArgumentNullException( nameof( factory ) );
            if( routes == null ) throw new ArgumentNullException( nameof( routes ) );

            _events = events;
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

            using( CommandSchedulingPipeline pipeline = new CommandSchedulingPipeline( scheduledOperation ) )
            {
                await new CommandFiltersInvoker( pipeline, _factory ).Invoke( _source.Token );
                await new CommandExecutor( pipeline, _factory, _executorSelector ).Invoke( _source.Token );
            }
        }

        public void Execute( IActivityMonitor monitor, ScheduledCommand operation )
        {
            Timer t = new Timer( TimerCallback, operation, (int) (operation.Scheduling.WhenCommandShouldBeRaised - DateTime.UtcNow ).TotalMilliseconds, Timeout.Infinite);
            _timers.Add( t );
        }
    }
}
