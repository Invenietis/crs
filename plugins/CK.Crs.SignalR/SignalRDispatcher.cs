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

        public void Broadcast<T>( Response<T> response ) => InvokeAsync( _hubContext.Clients.All, response );

        public void Send<T>( string callerId, Response<T> response ) => InvokeAsync( _hubContext.Clients.User( callerId ), response );

        private Task InvokeAsync<T>( IClientProxy clientProxy, Response<T> response )
            => clientProxy.InvokeAsync( "ReceiveResult", response.RequestId, response.ResponseType, response.Payload );

        public void Dispose() { }
    }
}
