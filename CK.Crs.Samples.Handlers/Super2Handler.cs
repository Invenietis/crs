using CK.Crs.Samples.Messages;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class Super2Handler : RequestHandlerAsync<Super2Command>
    {
        public override Task<Super2Command> HandleAsync(Super2Command command, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine("Super 2 - I'm Actor=" + command.ActorId + " on behalf of Actor=" + command.AuthenticatedActorId);
            return base.HandleAsync(command, cancellationToken);
        }
    }
}
