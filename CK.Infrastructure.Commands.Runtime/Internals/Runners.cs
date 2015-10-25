using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class Runners : ICommandRunner
    {
        private AsyncCommandRunner _asyncRunner;
        private CommandRunner _syncRunner;

        public Runners( ICommandHandlerFactory handlerFactory, ICommandResponseDispatcher eventDispatcher, IServiceProvider sp )
        {
            _syncRunner = new CommandRunner( handlerFactory, sp );
            _asyncRunner = new AsyncCommandRunner( _syncRunner, eventDispatcher );
        }

        public Task<ICommandResponse> RunAsync( CommandProcessingContext ctx, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            if( ctx.Request.CommandDescription.IsLongRunning )
            {
                return _asyncRunner.RunAsync( ctx, cancellationToken );
            }
            return _syncRunner.RunAsync( ctx, cancellationToken );
        }
    }
}
