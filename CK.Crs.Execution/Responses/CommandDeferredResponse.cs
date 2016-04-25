
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{
    class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse( CommandAction ctx ) : base( CommandResponseType.Asynchronous, ctx.CommandId )
        {
            Payload = ctx.CallbackId;
        }
    }
}
