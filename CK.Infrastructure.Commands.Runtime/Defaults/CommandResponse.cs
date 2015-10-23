using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public abstract class CommandResponse : ICommandResponse
    {
        public CommandResponse( CommandContext ctx )
        {
            CommandId = ctx.CommandId;
        }

        public CommandResponseType ResponseType { get; protected set; }

        public object Payload { get; protected set; }

        public Guid CommandId { get; private set; }
    }

    internal class DeferredResponse : CommandResponse
    {
        public DeferredResponse( string callbackId, CommandContext ctx ) : base( ctx )
        {
            ResponseType = CommandResponseType.Deferred;
            Payload = callbackId;
        }
    }

    internal class DirectResponse : CommandResponse
    {
        public DirectResponse( object result, CommandContext ctx ) : base( ctx )
        {
            ResponseType = CommandResponseType.Direct;
            Payload = result;
        }
    }

    internal class ErrorResponse : CommandResponse
    {
        public ErrorResponse( string msg, CommandContext ctx ) : base( ctx )
        {
            ResponseType = CommandResponseType.Error;
            Payload = msg;
        }
    }

}
