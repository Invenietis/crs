using CK.Core;
using System;

namespace CK.Crs
{
    public sealed class CommandFilterContext
    {
        private Response _response;

        public CommandFilterContext( object command, ICommandContext context, IEndpointModel model )
        {
            Command = command;
            CommandContext = context;
            Endpoint = model; 
        }

        public object Command { get; }

        public IActivityMonitor Monitor => CommandContext.Monitor;

        public ICommandContext CommandContext { get; }

        public IEndpointModel Endpoint { get; }

        public void SetResponse( Response response )
        {
            _response = response ?? throw new ArgumentNullException( nameof( response ) );
        }

        public Response Response => _response;
    }
}
