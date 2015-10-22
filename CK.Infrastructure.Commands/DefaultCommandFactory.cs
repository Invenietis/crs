using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using Microsoft.AspNet.Http;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandFactory : ICommandRequestFactory
    {
        public ICommandRequest CreateCommand( CommandRouteRegistration routeInfo, HttpRequest request )
        {
            object command = ReadBody( request.Body, routeInfo.CommandType );
            return new CommandRequest( command )
            {
                CallbackId = request.Query["c"],
                CommandServerType = routeInfo.CommandType,
                IsLongRunning = routeInfo.IsLongRunningCommand
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