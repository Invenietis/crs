using System;

namespace CK.Crs.Runtime
{
    public class CommandErrorResponse : CommandResponse
    {
        public CommandErrorResponse( string msg, Guid commandId ) : base( CommandResponseType.InternalError, commandId )
        {
            Payload = msg;
        }
    }

}
