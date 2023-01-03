namespace CK.Crs.Responses
{
    public class InvalidResponse : Response<object>
    {
        public InvalidResponse( string commandId, object reason ) : base( Crs.ResponseType.ValidationError, commandId, reason )
        {
        }
    }
}
