using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse( CommandExecutionContext ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Deferred;
            Payload = ctx.CallbackId;
        }
    }
}
