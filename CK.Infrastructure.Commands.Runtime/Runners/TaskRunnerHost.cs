using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    internal class TaskRunnerHost : ICommandRunnerHost
    {
        readonly ICommandResponseDispatcher _commandResponseDispatcher;

        public TaskRunnerHost( ICommandResponseDispatcher dispatcher )
        {
            if( dispatcher == null ) throw new ArgumentNullException( nameof( dispatcher ) );
            _commandResponseDispatcher = dispatcher;
        }

        public Task HostJob( IActivityMonitor monitor, ICommandRunner runner, CommandExecutionContext ctx, CancellationToken cancellation = default( CancellationToken ) )
        {
            var token = monitor.DependentActivity().CreateTokenWithTopic( GetType().Name );

            var t = new Task( async () =>
            {
                // We override the IActivityMonitor with a dependant one to be thread safe !
                using( var dependentMonitor = token.CreateDependentMonitor() )
                {
                    ctx.RuntimeContext.Monitor = dependentMonitor;
                    await runner.RunAsync( ctx);
                    await _commandResponseDispatcher.DispatchAsync( ctx.RuntimeContext.CallbackId, ctx.CreateResponse() );
                }
            } );

            t.Start( TaskScheduler.Current );
            return Task.FromResult( 0 );
        }
    }
}
