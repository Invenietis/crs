using System;

namespace CK.Crs
{
    public class CommandInvalidResponse : CommandResponse
    {
        public CommandInvalidResponse( Guid commandID, string reason ) : base( CommandResponseType.ValidationError, commandID )
        {
            Payload = reason;
        }
    }
}
