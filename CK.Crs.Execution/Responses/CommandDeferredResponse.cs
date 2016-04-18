﻿
using CK.Crs.Pipeline;

namespace CK.Crs.Runtime.Execution
{
    public class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse( CommandAction ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Deferred;
            Payload = ctx.CallbackId;
        }
    }
}
