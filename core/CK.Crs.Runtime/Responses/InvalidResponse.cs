using System;

namespace CK.Crs
{
    public class InvalidResponse : Response<object>
    {
        public InvalidResponse( Guid requestId, object reason ) : base(Crs.ResponseType.ValidationError, requestId )
        {
            Payload = reason;
        }
    }
}
