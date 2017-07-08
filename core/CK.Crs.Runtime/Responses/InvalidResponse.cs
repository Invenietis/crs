using System;

namespace CK.Crs
{
    public class InvalidResponse : Response
    {
        public InvalidResponse( Guid requestId, string reason ) : base(Crs.ResponseType.ValidationError, requestId )
        {
            Payload = reason;
        }
    }
}
