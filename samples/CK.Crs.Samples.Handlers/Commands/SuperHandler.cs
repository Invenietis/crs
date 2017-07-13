using CK.Crs.Samples.Messages;
using System;
using System.Threading.Tasks;
using System.Threading;
using CK.Core;

namespace CK.Crs.Samples.Handlers
{
    public class SuperHandler : ICommandHandler<SuperCommand, SuperCommand.Result>
    {
        IBus _dispatcher;
        public SuperHandler( IBus dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public async Task<SuperCommand.Result> HandleAsync( SuperCommand command, ICommandContext context )
        {
            var evt = new SuperEvent( context.CommandId, command.ActorId, command.AuthenticatedActorId )
            {
                Message = $"Super - I'm Actor={ command.ActorId} on behalf of Actor={ command.AuthenticatedActorId}"
            };

            context.Monitor.Trace( evt.Message );

            await _dispatcher.PublishAsync( evt, context );

            return new SuperCommand.Result( "Bouyah" );
        }
    }
}
