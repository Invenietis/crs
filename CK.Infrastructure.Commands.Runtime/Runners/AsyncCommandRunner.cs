using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class AsyncCommandRunner : ICommandRunner
    {
        readonly CommandRunner _localRunner;
        readonly ICommandResponseDispatcher _commandResponseDispatcher;

        public AsyncCommandRunner( CommandRunner innerRunner, ICommandResponseDispatcher dispatcher )
        {
            if( innerRunner == null ) throw new ArgumentNullException( nameof( innerRunner ) );
            _localRunner = innerRunner;
            _commandResponseDispatcher = dispatcher;
        }

        public Task<ICommandResponse> RunAsync( CommandProcessingContext ctx, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var newContext = ctx.CloneContext();
            var t = new Task( async () =>
            {
                var response =  await _localRunner.RunAsync( newContext, cancellationToken );
                await _commandResponseDispatcher.DispatchAsync( newContext.Request.CallbackId, response, cancellationToken );

            }, cancellationToken, TaskCreationOptions.LongRunning );

            t.Start( TaskScheduler.Current );
            return Task.FromResult( ctx.CreateDeferredResponse() );
        }
    }
}
