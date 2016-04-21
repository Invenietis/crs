using System;

namespace CK.Crs.Runtime
{
    public class CommandInvalidResponse : CommandResponse
    {
        public CommandInvalidResponse( Guid commandID, string reason ) : base( CommandResponseType.ValidationError, commandID )
        {
            Payload = reason;
        }
    }
}
