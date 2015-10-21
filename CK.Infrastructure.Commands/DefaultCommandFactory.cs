using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandFactory : ICommandRequestFactory
    {
        public ICommandRequest CreateCommand( CommandRouteRegistration routeInfo, Stream requestPayload )
        {
            object command = ReadBody( requestPayload, routeInfo.CommandType );
            return new CommandRequest( command )
            {
                CommandServerType = routeInfo.CommandType,
                CallbackId = null
            };
        }

        protected virtual object ReadBody( Stream requestPayload, Type commandType )
        {
            using( var reader = new StreamReader( requestPayload ) )
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject( json, commandType );
            }
        }
    }
}