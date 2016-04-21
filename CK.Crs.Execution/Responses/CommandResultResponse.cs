using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{
    public class CommandResultResponse : CommandResponse
    {
        public CommandResultResponse( object result, CommandAction ctx ) : base( CommandResponseType.Synchronous, ctx.CommandId )
        {
            Payload = result;
        }
    }
}
