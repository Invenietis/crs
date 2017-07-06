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
            context.Monitor.Trace().Send( "Super - I'm Actor=" + command.ActorId + " on behalf of Actor=" + command.AuthenticatedActorId);

            var evt = new SuperCommandHandledEvent(command.Id, command.ActorId, command.AuthenticatedActorId);
            await _dispatcher.PublishAsync( evt, context );

            return null;
        }
    }
}
