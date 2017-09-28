using System;

namespace CK.Crs
{
    public class ErrorResponse : Response<object>
    {
        public ErrorResponse( Exception ex, string commandId ) : base( Crs.ResponseType.InternalError, commandId )
        {
            Payload = ex.ToString();
        }
        public ErrorResponse( string msg, string commandId ) : base( Crs.ResponseType.InternalError, commandId )
        {
            Payload = msg;
        }
    }

}
