using CK.Crs.Pipeline;

namespace CK.Crs.Runtime.Execution
{
    public class CommandResultResponse : CommandResponse
    {
        public CommandResultResponse( object result, CommandAction ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Direct;
            Payload = result;
        }
    }
}
