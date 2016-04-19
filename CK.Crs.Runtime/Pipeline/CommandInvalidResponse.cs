using System;

namespace CK.Crs.Runtime
{
    public class CommandInvalidResponse : CommandResponse
    {
        public CommandInvalidResponse( Guid commandID, string reason ) : base( commandID )
        {
            ResponseType = CommandResponseType.PreConditionFailed;
            Payload = reason;
        }
    }
}
