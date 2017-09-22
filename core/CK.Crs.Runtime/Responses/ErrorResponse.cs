using System;

namespace CK.Crs
{
    public class ErrorResponse : Response<object>
    {
        public ErrorResponse( Exception ex, Guid commandId ) : base( Crs.ResponseType.InternalError, commandId )
        {
            Payload = ex.ToString();
        }
        public ErrorResponse( string msg, Guid commandId ) : base( Crs.ResponseType.InternalError, commandId )
        {
            Payload = msg;
        }
    }

}
