using System;

namespace CK.Crs
{
    public class CommandErrorResponse : CommandResponse
    {
        public CommandErrorResponse(Exception ex, Guid commandId) : base(CommandResponseType.InternalError, commandId)
        {
            Payload = ex.ToString();
        }
        public CommandErrorResponse( string msg, Guid commandId ) : base( CommandResponseType.InternalError, commandId )
        {
            Payload = msg;
        }
    }

}
