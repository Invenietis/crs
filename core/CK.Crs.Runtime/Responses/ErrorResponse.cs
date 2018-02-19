using System;

namespace CK.Crs.Responses
{
    public class ErrorResponse : Response<string>
    {
        public ErrorResponse( Exception ex, string commandId ) : base( Crs.ResponseType.InternalError, commandId, ex.ToString() )
        {
        }

        public ErrorResponse( string msg, string commandId ) : base( Crs.ResponseType.InternalError, commandId, msg )
        {
        }
    }

}
