using System;

namespace CK.Crs
{
    public class InvalidResponse : Response
    {
        public InvalidResponse( Guid requestId, object reason ) : base(Crs.ResponseType.ValidationError, requestId )
        {
            Payload = reason;
        }
    }
}
