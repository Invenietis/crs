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
        ICommandDispatcher _dispatcher;
        public SuperHandler( ICommandDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        protected override async Task<object> HandleCommandAsync(SuperCommand command, ICommandContext context )
        {
            var evt = new SuperCommandHandledEvent(command.Id, command.ActorId, command.AuthenticatedActorId)
            {
                Message = "Super - I'm Actor=" + command.ActorId + " on behalf of Actor=" + command.AuthenticatedActorId
            };

            context.Monitor.Trace().Send( evt.Message );

            await _dispatcher.PublishAsync( evt, context );

            return null;
        }
    }
}
