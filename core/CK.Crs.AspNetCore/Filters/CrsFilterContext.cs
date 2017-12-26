using CK.Core;
using Microsoft.AspNetCore.Http;
using System;

namespace CK.Crs.AspNetCore
{
    public class CrsFilterContext
    {
        private Response _response;

        public CrsFilterContext( object command, ICommandContext context, IEndpointModel model, HttpContext httpContext )
        {
            Command = command;
            Context = context;
            Model = model;
            HttpContext = httpContext;
        }

        public object Command { get; }
        public IActivityMonitor Monitor => Context.Monitor;

        public ICommandContext Context { get; }

        public IEndpointModel Model { get; }

        public HttpContext HttpContext { get; }

        public void SetResponse( Response response )
        {
            _response = response ?? throw new ArgumentNullException( nameof( response ) );
        }

        public Response Response => _response;
    }
}
