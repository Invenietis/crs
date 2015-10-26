using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class AsyncCommandRunner : ICommandRunnerHost
    {
        readonly ICommandResponseDispatcher _commandResponseDispatcher;

        public AsyncCommandRunner( ICommandResponseDispatcher dispatcher )
        {
            if( dispatcher == null ) throw new ArgumentNullException( nameof( dispatcher ) );
            _commandResponseDispatcher = dispatcher;
        }

        public Task HostJob( ICommandRunner runner, CommandExecutionContext ctx, CancellationToken cancellation = default( CancellationToken ) )
        {
            var t = new Task( async () =>
            {
                await runner.RunAsync( ctx);
                await _commandResponseDispatcher.DispatchAsync( ctx.RuntimeContext.CallbackId, ctx.CreateResponse() );
            } );

            t.Start( TaskScheduler.Current );
            return Task.FromResult( 0 );
        }
    }
}
