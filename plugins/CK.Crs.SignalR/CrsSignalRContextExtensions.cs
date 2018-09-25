using Microsoft.AspNetCore.SignalR;

namespace CK.Crs
{
    public static class CrsSignalRContextExtensions
    {
        public static readonly string CallerIdProtocol = "SignalR";

        public static string GetConnectionId( this CallerId callerId )
        {
            if( callerId.Protocol == CallerIdProtocol ) return callerId.Values[0];
            return null;
        }

        public static string GetUserName( this CallerId callerId )
        {
            if( callerId.Protocol == CallerIdProtocol ) return callerId.Values[1];
            return null;
        }

        public static CallerId GetCallerId( this HubCallerContext context )
        {
            var values = new[] { context.ConnectionId, context.User.Identity.Name };
            return new CallerId( CallerIdProtocol, values );
        }
    }
}
