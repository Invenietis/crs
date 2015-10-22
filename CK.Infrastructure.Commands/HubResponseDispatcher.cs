using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace CK.Infrastructure.Commands
{
    public class HubResponseDispatcher : ICommandResponseDispatcher
    {
        readonly IConnectionManager _connectionManager;

        public HubResponseDispatcher( IConnectionManager connectionManager )
        {
            _connectionManager = connectionManager;
        }

        public Task DispatchAsync( string callbackId, ICommandResponse response )
        {
            var hubContext = _connectionManager.GetHubContext<CommandResponseHub, ICommandResponseClient>();
            return hubContext.Clients.Client( callbackId ).ReceiveCommandResponse( response.Payload );
        }
    }
}
