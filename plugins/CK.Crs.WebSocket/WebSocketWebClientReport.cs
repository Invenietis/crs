using CK.Communication.WebSockets;
using CK.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{

    public class WebSocketWebClientDispatcher : IWebClientDispatcher, IDisposable
    {
        readonly IWebSocketSender _sender;
        readonly IWebSocketConnectedClients _connectedClients;
        readonly BlockingCollection<WebSocketMessage> _messages;
        readonly ILiveEventStore _liveEventStore;

        public WebSocketWebClientDispatcher( ILiveEventStore liveEventStore, IWebSocketSender sender, IWebSocketConnectedClients connectedClients )
        {
            _liveEventStore = liveEventStore;
            _sender = sender;
            _connectedClients = connectedClients;
            _messages = new BlockingCollection<WebSocketMessage>();

            Task.Run( async () =>
             {
                 var monitor = new ActivityMonitor( "WebSocketWebClientDispatcher" );

                 while( !_messages.IsCompleted )
                 {
                     var msg = _messages.Take();
                     if( msg == null ) continue;
                     using( monitor.StartDependentActivity( msg.Token ) )
                     {
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
                 }
             } );
        }

        public async Task Send<T>( string eventName, T message, ICommandContext context )
        {
            await DoSendToClient( context.CallerId, eventName, message, context );
        }

        public async Task Broadcast<T>( string eventName, T message, ICommandContext context )
        {
            foreach( var client in _connectedClients.Connections )
            {
                await DoSendToClient( client, eventName, message, context );
            }
        }
        private async Task DoSendToClient<T>( string clientId, string eventName, T message, ICommandContext context )
        {
            if( !context.ReceiverModel.SupportsClientEventsFiltering || await _liveEventStore.IsRegistered( context.CallerId, eventName ) )
            {
                var msg = Serialize( message, context );
                _messages.Add( new WebSocketMessage( msg, context.CallerId, context ) );
            }
        }

        string Serialize<T>( T message, ICommandContext context )
        {
            var response = new WebSocketCommandResponse<T>( message, ResponseType.Synchronous, context.CommandId );
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

            public Guid CommandId { get; }

            public ActivityMonitor.DependentToken Token { get; }

            public WebSocketMessage( string message, string clientId, ICommandContext context )
            {
                CommandId = context.CommandId;
                ClientId = context.CallerId;
                Message = message;
                Token = context.Monitor.DependentActivity().CreateToken();
            }
        }

        class WebSocketCommandResponse<T> : Response<T>
        {
            public WebSocketCommandResponse( T message, ResponseType responseType, Guid commandId ) : base( responseType, commandId )
            {
                Payload = message;
            }
        }
    }
}
