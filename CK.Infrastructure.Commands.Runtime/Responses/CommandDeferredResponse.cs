using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse( CommandContext ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Deferred;
            Payload = ctx.CallbackId;
        }
    }
}
