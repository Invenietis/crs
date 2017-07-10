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

        public WebSocketWebClientDispatcher( IWebSocketSender sender, IWebSocketConnectedClients connectedClients )
        {
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
                             if( String.IsNullOrEmpty( msg.CallerId ) )
                             {
                                 monitor.Trace( "Broadcasting message to all connected clients." );
                                 await _sender.Send( msg.ToString() );
                             }
                             else
                             {
                                 var client = _connectedClients.Connections.SingleOrDefault( c => c.Equals( msg.CallerId, StringComparison.OrdinalIgnoreCase ) );
                                 if( !String.IsNullOrEmpty( client ) )
                                 {
                                     monitor.Trace( $"Sending message to client {msg.CallerId}." );
                                     await _sender.Send( client, msg.ToString() );
                                 }
                                 else
                                 {
                                     monitor.Warn( $"Client  {msg.CallerId} not actualy found." );
                                 }
                             }
                         }
                         catch( Exception ex )
                         {
                             monitor.Error( ex );
                         }
                     }
                 }
             } );
        }

        public void Send<T>( T message, ICommandContext context )
        {
            var msg = Serialize( message, context );
            _messages.Add( new WebSocketMessage( msg, context, false ) );
        }

        public void Broadcast<T>( T message, ICommandContext context )
        {
            var msg = Serialize( message, context );
            _messages.Add( new WebSocketMessage( msg, context, true ) );
        }

        string Serialize<T>( T message, ICommandContext context )
        {
            var response = new WebSocketCommandResponse<T>( message, ResponseType.Synchronous, context.Id );
            return Newtonsoft.Json.JsonConvert.SerializeObject( response );
        }

        public void Dispose()
        {
            _messages.CompleteAdding();
            _messages.Dispose();
        }


        class WebSocketMessage
        {
            public string CallerId { get; }

            public string Message { get; }

            public Guid CommandId { get; }

            public ActivityMonitor.DependentToken Token { get; }

            public WebSocketMessage( string message, ICommandContext context, bool broadcast = false )
            {
                CommandId = context.Id;
                CallerId = broadcast ? null : context.CallerId;
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
