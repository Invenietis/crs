using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    internal class OutProcessCommandExecutor : CommandExecutor
    {
        readonly ICommandResponseDispatcher _commandResponseDispatcher;

        public OutProcessCommandExecutor( ICommandResponseDispatcher dispatcher, ICommandReceiverFactories factories ) : base( factories )
        {
            if( dispatcher == null ) throw new ArgumentNullException( nameof( dispatcher ) );
            _commandResponseDispatcher = dispatcher;
        }

        public override Task ExecuteAsync( CommandExecutionContext ctx, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var token = ctx.RuntimeContext.Monitor.DependentActivity().CreateTokenWithTopic( GetType().Name );

            // This implementation does not guarantee that the command will be correctly handled...
            // We need some retry mechanism and a pending command persistence mechanism to be resilient.
            var t = new Task( async () =>
            {
                // We override the IActivityMonitor with a dependant one to be thread safe !
                using( var dependentMonitor = token.CreateDependentMonitor() )
                {
                    ctx.RuntimeContext.Monitor = dependentMonitor;
                    await base.ExecuteAsync( ctx, cancellationToken );
                    await _commandResponseDispatcher.DispatchAsync( ctx.RuntimeContext.CallbackId, ctx.CreateResponse() );
                }
            } );

            t.Start( TaskScheduler.Current );
            return Task.FromResult( 0 );
        }
    }
}
