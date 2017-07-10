using CK.Crs.Samples.Messages;
using Paramore.Brighter;
using System;
using System.Threading.Tasks;
using System.Threading;
using CK.Core;

namespace CK.Crs.Samples.Handlers
{
    public class SuperHandler : CommandHandlerAsync<SuperCommand>
    {
        IBus _dispatcher;
        public SuperHandler( IBus dispatcher )
        {
            _dispatcher = dispatcher;
        }

        protected override async Task HandleCommandAsync(SuperCommand command, ICommandContext context )
        {
            var evt = new SuperEvent(command.Id, command.ActorId, command.AuthenticatedActorId)
            {
                Message = $"Super - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}"
            };

            context.Monitor.Trace( evt.Message );

            await _dispatcher.PublishAsync( evt, context );
        }
    }
}
