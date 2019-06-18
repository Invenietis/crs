using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    class CrsHub : Hub<ICrsHub>
    {
        private readonly ICrsConnectionManager _connectionManager;

        public CrsHub( ICrsConnectionManager connectionManager )
        {
            _connectionManager = connectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var callerId = Context.GetCallerId();
            await _connectionManager.AddConnection( callerId );
            await Clients.Client( Context.ConnectionId ).ReceiveCallerId( callerId.ToString() );
        }

        public override Task OnDisconnectedAsync( Exception exception )
        {
            var callerId = Context.GetCallerId();
            return _connectionManager.RemoveConnection( callerId );
        }
    }
}
