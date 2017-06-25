using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{
    /// <summary>
    /// Represents the S of VISAM : Synchronous Response
    /// </summary>
    class CommandResultResponse : CommandResponse
    {
        public CommandResultResponse( object result, CommandAction ctx ) : base( CommandResponseType.Synchronous, ctx.CommandId )
        {
            Payload = result;
        }
    }
}
