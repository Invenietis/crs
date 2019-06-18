using CK.Core;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    class SignalRDispatcher : IResultDispatcher
    {
        // TODO: When typedHub will be available
        // private readonly IHubContext<CrsHub, ICrsHub> _hubContext;
        private readonly IHubContext<CrsHub, ICrsHub> _hubContext;

        public SignalRDispatcher( IHubContext<CrsHub, ICrsHub> hubContext )
        {
            _hubContext = hubContext;
        }

        public Task Broadcast<T>( ICommandContext context, Response<T> response )
        {
            context.Monitor.Trace( $"Broadcasting response to client {context.CallerId.GetUserName()}." );
            return InvokeAsync( _hubContext.Clients.User( context.CallerId.GetUserName() ), response );
        }

        public Task Send<T>( ICommandContext context, Response<T> response )
        {
            context.Monitor.Trace( $"Sending response to client {context.CallerId.GetConnectionId()}." );
            return InvokeAsync( _hubContext.Clients.Client( context.CallerId.GetConnectionId() ), response );
        }

        private Task InvokeAsync<T>( ICrsHub clientProxy, Response<T> response )
            => clientProxy.ReceiveResult( response.CommandId, response.ResponseType, response.Payload );

        public void Dispose() { }
    }
}
