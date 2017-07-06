using CK.Core;
using CK.Crs.Samples.Messages;
using Paramore.Brighter;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class Super2Handler : CommandHandlerAsync<Super2Command>
    {
        protected override Task<object> HandleCommandAsync(Super2Command command, ICommandContext context )
        {
            context.Monitor.Trace().Send( "Super 2 - I'm Actor=" + command.ActorId + " on behalf of Actor=" + command.AuthenticatedActorId);

            return Task.FromResult<object>( null );
        }
    }
}
