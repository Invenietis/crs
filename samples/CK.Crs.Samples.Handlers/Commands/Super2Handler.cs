using CK.Core;
using CK.Crs.Samples.Messages;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class Super2Handler : ICommandHandler<Super2Command>
    {
        public Task HandleAsync( Super2Command command, ICommandContext context )
        {
            context.Monitor.Trace( $"Super 2 - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}" );

            return Task.CompletedTask;
        }
    }
}
