
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{
    /// <summary>
    /// Represent the A of VISAM : Asynchronous response.
    /// </summary>
    class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse( CommandAction ctx ) : base( CommandResponseType.Asynchronous, ctx.CommandId )
        {
            Payload = ctx.CallbackId;
        }
    }
}
