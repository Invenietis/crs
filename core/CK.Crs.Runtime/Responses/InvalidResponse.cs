using System;

namespace CK.Crs
{
    public class InvalidResponse : Response<object>
    {
        public InvalidResponse( string commandId, object reason ) : base( Crs.ResponseType.ValidationError, commandId )
        {
            Payload = reason;
        }
    }
}
