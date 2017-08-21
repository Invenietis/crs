using CK.Core;
using CK.Crs.Samples.Messages;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class InProcessHandler : ICommandHandler<QueuedCommand>, ICommandHandler<SyncCommand, SyncCommand.Result>
    {
        public Task HandleAsync( QueuedCommand command, ICommandContext context )
        {
            context.Monitor.Trace( $"Queued Command - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}" );

            return Task.CompletedTask;
        }

        public Task<SyncCommand.Result> HandleAsync( SyncCommand command, ICommandContext context )
        {
            context.Monitor.Trace( $"Sync Command - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}" );
            return Task.FromResult( new SyncCommand.Result
            {
                Message = "Bouyahh !!"
            } );
        }
    }
}
