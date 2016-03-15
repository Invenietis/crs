using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse( ICommandExecutionContext ctx ) : base( ctx.Action.CommandId )
        {
            ResponseType = CommandResponseType.Deferred;
            Payload = ctx.Action.CallbackId;
        }
    }
}
