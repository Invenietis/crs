using CK.Crs.Samples.Messages;
using System;
using System.Threading.Tasks;
using System.Threading;
using CK.Core;


namespace CK.Crs.Samples.Handlers
{
    public class RemoteHandler : ICommandHandler<RemotelyQueuedCommand, RemotelyQueuedCommand.Result>
    {
        public async Task<RemotelyQueuedCommand.Result> HandleAsync( RemotelyQueuedCommand command, ICommandContext context )
        {
            var evt = new SuperEvent( context.CommandId, command.ActorId, command.AuthenticatedActorId )
            {
                Message = $"Super - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}"
            };

            context.Monitor.Trace( evt.Message );

            return new RemotelyQueuedCommand.Result( "Bouyah" );
        }
    }
}
