using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    public class SignalRDispatcher : IClientDispatcher
    {
        private readonly IHubContext<CrsHub> _hubContext;

        public SignalRDispatcher( IHubContext<CrsHub> hubContext )
        {
            _hubContext = hubContext;
        }

        public void Broadcast( Response response ) => InvokeAsync( _hubContext.Clients.All, response );

        public void Send( string callerId, Response response ) => InvokeAsync( _hubContext.Clients.User( callerId ), response );

        private Task InvokeAsync( IClientProxy clientProxy, Response response )
            => clientProxy.InvokeAsync( "ReceiveResult", response.RequestId, response.ResponseType, response.Payload );

        public void Dispose() { }
    }
}
