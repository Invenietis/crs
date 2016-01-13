using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class DeferredResponse : CommandResponse
    {
        public DeferredResponse( CommandContext ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Deferred;
            Payload = ctx.CallbackId;
        }
    }
}
