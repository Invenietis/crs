using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class AsyncCommandRunner : ICommandRunner
    {
        CommandRunner _localRunner;
        public ICommandResponseDispatcher EventDispatcher { get; private set; }

        public AsyncCommandRunner( ICommandHandlerFactory factory, ICommandResponseDispatcher dispatcher )
        {
            _localRunner = new CommandRunner( factory );
            EventDispatcher = dispatcher;
        }

        public Task ProcessAsync( CommandContext ctx )
        {
            var newContext = ctx.CloneContext();
            var t = new Task( async () =>
            {
                await _localRunner.ProcessAsync( newContext );
                await EventDispatcher.DispatchAsync( newContext.Request.CallbackId, newContext.Response );

            }, TaskCreationOptions.LongRunning );

            ctx.CreateDeferredResponse();
            t.Start( TaskScheduler.Current );
            return Task.FromResult( 0 );
        }
    }
}
