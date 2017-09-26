using CK.Communication.WebSockets;
using CK.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{

    public class WebSocketWebClientDispatcher : IResultDispatcher, IDisposable
    {
        readonly IWebSocketSender _sender;
        readonly IClientEventStore _liveEventStore;
        readonly IWebSocketConnectedClients _connectedClients;
        readonly BlockingCollection<WebSocketMessage> _messages;
        readonly IOptions<ClientDispatcherOptions> _dispatcherOptions;

        public WebSocketWebClientDispatcher(
            IClientEventStore liveEventStore,
            IWebSocketSender sender,
            IWebSocketConnectedClients connectedClients,
            IOptions<ClientDispatcherOptions> dispatcherOptions )
        {
            _liveEventStore = liveEventStore;
            _sender = sender;
            _connectedClients = connectedClients;
            _messages = new BlockingCollection<WebSocketMessage>();
            _dispatcherOptions = dispatcherOptions;

            Task.Run( async () =>
             {
                 var monitor = new ActivityMonitor( "WebSocketWebClientDispatcher" );

                 while( !_messages.IsCompleted )
                 {
                     var msg = _messages.Take();
                     if( msg == null ) continue;
                     try
                     {
                         monitor.Trace( $"Sending message to client {msg.ClientId}." );
                         await _sender.Send( msg.ClientId, msg.ToString() );
                     }
                     catch( Exception ex )
                     {
                         monitor.Error( ex );
                     }
                 }
             } );
        }

        public void Send<T>( ICommandContext context, Response<T> response )
        {
            DoSendToClient( null, response );
        }

        public void Broadcast<T>( ICommandContext context, Response<T> response )
        {
            foreach( var client in _connectedClients.Connections )
            {
                DoSendToClient( client, response );
            }
        }
        private void DoSendToClient<T>( string clientId, Response<T> response )
        {
            if( !_dispatcherOptions.Value.SupportsServerSideEventsFiltering )
            {
                var msg = Serialize( response );
                _messages.Add( new WebSocketMessage( msg, clientId ) );
            }
        }

        string Serialize( Response response )
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject( response );
        }

        public void Dispose()
        {
            _messages.CompleteAdding();
            _messages.Dispose();
        }

        class WebSocketMessage
        {
            public string ClientId { get; }

            public string Message { get; }

            public WebSocketMessage( string message, string clientId )
            {
                ClientId = clientId;
                Message = message;
            }
        }

        class WebSocketCommandResponse<T> : Response<T>
        {
            public WebSocketCommandResponse( T message, ResponseType responseType, string commandId ) : base( responseType, commandId )
            {
                Payload = message;
            }
        }
    }
}
