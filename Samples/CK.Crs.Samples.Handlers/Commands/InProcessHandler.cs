using CK.Core;
using CK.Crs.Samples.Messages;

using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class InProcessHandler : ICommandHandler<QueuedCommand, QueuedCommand.Result>, ICommandHandler<SyncCommand, SyncCommand.Result>
    {
        public Task<QueuedCommand.Result> HandleAsync( QueuedCommand command, ICommandContext context )
        {
            context.Monitor.Trace( $"Queued Command - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}" );

            return Task.FromResult( new QueuedCommand.Result
            {
                Message = "Bouyahh Async !!"
            } );
        }

        public async Task<SyncCommand.Result> HandleAsync( SyncCommand command, ICommandContext context )
        {
            context.Monitor.Trace( $"Sync Command - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}" );
            return new SyncCommand.Result
            {
                Message = "Bouyahh !!"
            };
        }
    }
}
