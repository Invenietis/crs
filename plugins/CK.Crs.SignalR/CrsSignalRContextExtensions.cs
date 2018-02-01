using CK.Crs;
using CK.Crs.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;

namespace CK.Crs
{
    public static class CrsSignalRContextExtensions
    {
        public static readonly string CallerIdProtocol = "SignalR";

        public static string GetUserName( this CallerId callerId )
        {
            if( callerId.Protocol == CallerIdProtocol ) return callerId.Values[0];
            return null;
        }

        public static string GetConnectionId( this CallerId callerId )
        {
            if( callerId.Protocol == CallerIdProtocol ) return callerId.Values[1];
            return null;
        }

        public static CallerId GetCallerId( this HubCallerContext context )
        {
            var values = new[] { context.User.Identity.Name, context.ConnectionId };
            return new CallerId( CallerIdProtocol, values );
        }
    }
}
