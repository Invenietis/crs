using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class AsyncCommandRunner : ICommandRunner
    {
        CommandRunner _localRunner;
        public ICommandResponseDispatcher EventDispatcher { get; private set; }

        public AsyncCommandRunner( CommandRunner innerRunner, ICommandResponseDispatcher dispatcher )
        {
            _localRunner = innerRunner;
            EventDispatcher = dispatcher;
        }

        public Task<ICommandResponse> RunAsync( CommandProcessingContext ctx, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var newContext = ctx.CloneContext();
            var t = new Task( async () =>
            {
                var response =  await _localRunner.RunAsync( newContext, cancellationToken );
                await EventDispatcher.DispatchAsync( newContext.Request.CallbackId, response, cancellationToken );

            }, cancellationToken, TaskCreationOptions.LongRunning );

            t.Start( TaskScheduler.Current );
            return Task.FromResult( ctx.CreateDeferredResponse() );
        }
    }
}
