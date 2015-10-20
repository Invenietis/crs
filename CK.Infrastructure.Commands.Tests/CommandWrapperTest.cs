using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CK.Infrastructure.Commands.Tests
{
    public class TransferAmountCommand
    {
        public Guid SourceAccountId { get; set; }

        public Guid DestinationAccountId { get; set; }

        public decimal Amount { get; set; }
    }

    public class AmountTransferredEvent
    {
        public DateTime EffectiveDate { get; set; }

        public DateTime CancellableDate { get; set; }
    }

    public class AmountTransferFailedEvent
    {
        public DateTime FailedDate { get; set; }

        public string Reason { get; set; }
    }

    public class TransferAlwaysSuccessHandler : CommandHandler<TransferAmountCommand>
    {
        public override Task<object> DoHandleAsync( TransferAmountCommand command )
        {
            var result = new AmountTransferredEvent
            {
                EffectiveDate = DateTime.UtcNow.Date.AddDays( 2 ),
                CancellableDate = DateTime.UtcNow.AddHours( 1 )
            };
            return Task.FromResult<object>( result );
        }
    }

    [Collection( "CK.Infrastructure.Commands.Tests collection" )]
    public class CommandWrapperTest
    {
        public readonly ITestOutputHelper Output;
        public CommandWrapperTest( ITestOutputHelper output )
        {
            Output = output;
        }

        [Fact]
        public async Task SendCommandAndWaitForEvents()
        {
            string serverAddress = "http://MyDumbServer/c/";
            using( ICommandServer server = new DumbServer( serverAddress ) )
            {
                // Server initialization
                server.RegisterHandler<TransferAmountCommand>( new TransferAlwaysSuccessHandler() );
                server.Run();

                // Client Code
                IClientCommandSender sender = new ClientCommandSender();

                TransferAmountCommand command = new TransferAmountCommand
                {
                    DestinationAccountId = Guid.NewGuid(),
                    SourceAccountId = Guid.NewGuid(),
                    Amount = 400
                };

                ClientCommandResult result = await sender.SendAsync(serverAddress, command );
                await result.OnResultAsync<AmountTransferredEvent>( @event =>
                {
                    Output.WriteLine( @event.CancellableDate.ToString() );
                    Output.WriteLine( @event.EffectiveDate.ToString() );
                    return Task.FromResult( 0 );
                } );
                await result.OnResultAsync<AmountTransferFailedEvent>( @event =>
                {
                    Output.WriteLine( @event.FailedDate.ToString() );
                    Output.WriteLine( @event.Reason );
                    return Task.FromResult( 0 );
                } );

                await result.OnErrorAsync( ex =>
                {
                    Console.WriteLine( ex.ToString() );
                    return Task.FromResult( 0 );
                } );

                await Task.Delay( TimeSpan.FromSeconds( 15 ) );
            }
        }
    }

    #region Client SDK

    public interface IClientCommandSender
    {
        Task<ClientCommandResult> SendAsync<T>( string address, T command );
    }

    public class ClientCommandSender : IClientCommandSender
    {
        public ClientCommandSender()
        {
        }

        public async Task<ClientCommandResult> SendAsync<T>( string address, T command )
        {
            CommandRequest request = new CommandRequest( command )
            {
                CallbackId = EventChannel.Instance.ConnectionId,
                CommandServerType = typeof( T)
            };
            ClientCommandResult result = null;
            try
            {
                IResponse response = await CommandChannel.SendAsync( address, request );
                if( response.ResponseType == 0 )
                {
                    result = new DeferredResult();
                    result.Result = request.CallbackId;
                }
                else if( response.ResponseType == 1 )
                {
                    result = new DirectResult();
                    result.Result = response.Payload;
                }
                else throw new InvalidOperationException();
                result.CommandId = response.CommandId;
                result.IsSuccess = response.ResponseType >= 0;
            }
            catch( Exception ex )
            {
                result = new DirectResult();
                result.IsSuccess = false;
            }
            return result;
        }
    }

    public abstract class ClientCommandResult
    {
        public Guid CommandId { get; set; }

        public bool IsFailure { get; set; }

        public bool IsSuccess { get; set; }

        public object Result { get; set; }

        public abstract Task OnResultAsync<TEvent>( Func<TEvent, Task> callback ) where TEvent : class;

        public abstract Task OnErrorAsync( Func<Exception, Task> exCallback );
    }

    public class DirectResult : ClientCommandResult
    {
        public override async Task OnResultAsync<TEvent>( Func<TEvent, Task> callback )
        {
            TEvent evt = Result as TEvent;
            if( evt != null ) await callback( evt );
        }
        public override async Task OnErrorAsync( Func<Exception, Task> exCallback )
        {
            Exception ex = Result as Exception;
            if( ex != null ) await exCallback( ex );
        }
    }

    public class DeferredResult : ClientCommandResult
    {
        public override Task OnResultAsync<TEvent>( Func<TEvent, Task> callback )
        {
            return EventChannel.Instance.ListenAsync( CommandId, (string)Result, callback );
        }

        public override Task OnErrorAsync( Func<Exception, Task> exCallback )
        {
            return EventChannel.Instance.ListenAsync( CommandId, (string)Result, exCallback );
        }
    }

    #endregion

    #region Abstractions

    public interface IRequest
    {
        object Command { get; }

        string CallbackId { get; }

        Type CommandServerType { get; }
    }

    public interface IResponse
    {
        Guid CommandId { get; }

        int ResponseType { get; }

        object Payload { get; }
    }
    public interface ICommandServer : IDisposable
    {
        void RegisterHandler<T>( ICommandHandler handler );

        Task<IResponse> ProcessCommandAsync( CommandRequest command );
        void Run();
    }

    public class CommandRequest : IRequest
    {
        public CommandRequest( object command )
        {
            Command = command;
        }

        public object Command { get; set; }

        public string CallbackId { get; set; }
        public Type CommandServerType { get; set; }
    }

    public class CommandResponse : IResponse
    {
        public Guid CommandId { get; set; }

        public int ResponseType { get; set; }

        public object Payload { get; set; }
    }

    public abstract class CommandHandler<T> : ICommandHandler where T : class
    {
        public Task<object> HandleAsync( object command )
        {
            return DoHandleAsync( command as T );
        }

        public abstract Task<object> DoHandleAsync( T command );
    }

    #endregion

    #region Commands and Events Channels

    public static class CommandChannel
    {
        internal static Task<IResponse> SendAsync( string serverAddress, CommandRequest request )
        {
            return Servers.GetServer( serverAddress ).ProcessCommandAsync( request );
        }
    }

    public struct PendingCommandInfo : IEquatable<PendingCommandInfo>
    {
        public Guid CommandId;

        public string CallbackId;

        public PendingCommandInfo( Guid commandId, string callbackId )
        {
            CommandId = commandId;
            CallbackId = callbackId;
        }

        public bool Equals( PendingCommandInfo other )
        {
            return other.CallbackId.Equals( CallbackId ) && other.CommandId.Equals( CommandId );
        }

        public override int GetHashCode()
        {
            return CommandId.GetHashCode() ^ CallbackId.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            return Equals( (PendingCommandInfo)obj );
        }
    }

    public class EventChannel
    {
        public static EventChannel Instance = new EventChannel();

        /// <summary>
        /// The unique connection identifier for this instance
        /// </summary>
        public string ConnectionId { get; } = Guid.NewGuid().ToString( "N" );

        /// <summary>
        /// Stop listen to events after the configured timeout value
        /// </summary>
        public TimeSpan DefaultTimeout { get; } = TimeSpan.FromMinutes( 1 );

        /// <summary>
        /// A list of pending events linked to this connection
        /// </summary>
        public ConcurrentDictionary<PendingCommandInfo, object> PendingEvents { get; } = new ConcurrentDictionary<PendingCommandInfo, object>();

        internal Task ListenAsync<TEvent>( Guid commandId, string callbackId, Func<TEvent, Task> callback ) where TEvent : class
        {
            return Task.Factory.StartNew( async () =>
            {
                bool shouldContinue = true;
                DateTime stopReceiving = DateTime.UtcNow.Add(DefaultTimeout);
                PendingCommandInfo key = new PendingCommandInfo( commandId, callbackId );

                // Message pump
                while( DateTime.UtcNow < stopReceiving && shouldContinue )
                {
                    object v;
                    if( PendingEvents.TryGetValue( key, out v ) )
                    {
                        if( v != null && v is TEvent )
                        {
                            if( PendingEvents.TryRemove( key, out v ) )
                            {
                                try
                                {
                                    await callback( v as TEvent );
                                }
                                finally
                                {
                                    shouldContinue = false;
                                }
                            }
                        }
                    }
                    await Task.Delay( 200 );
                }
            } );
        }

        public void Dispatch( string callbackId, CommandResponse response )
        {
            if( String.IsNullOrEmpty( callbackId ) ) throw new ArgumentNullException( nameof( callbackId ) );
            if( response == null ) throw new ArgumentNullException( nameof( response ) );

            var key = new PendingCommandInfo( response.CommandId, callbackId);
            PendingEvents.TryAdd( key, response.Payload );
        }
    }
    #endregion

    #region Server Command Receiver

    class DumbServer : ICommandServer, IDisposable
    {
        bool _disposed = false;
        public DumbServer( string serverAddress )
        {
            Servers.Register( serverAddress, this );
        }

        class PendingRequest
        {
            public CommandRequest CommandRequest { get; set; }

            public TaskCompletionSource<IResponse> CommandResponsePromise { get; set; }
        }

        Dictionary<Type, ICommandHandler> HandlerMap { get; } = new Dictionary<Type, ICommandHandler>();

        BlockingCollection<PendingRequest> IncomingRequests { get; } = new BlockingCollection<PendingRequest>();
        public void Run()
        {
            Task.Factory.StartNew( () =>
            {
                while( !_disposed )
                {
                    PendingRequest request = IncomingRequests.Take();
                    if( request == null ) continue;

                    CommandWrapper w  = new CommandWrapper();
                    w.CommandId = Guid.NewGuid();
                    w.CallbackId = request.CommandRequest.CallbackId;
                    w.CommandServerType = request.CommandRequest.CommandServerType;
                    w.CommandBody = request.CommandRequest.Command;

                    var response = new CommandResponse();
                    response.CommandId = w.CommandId;

                    var handler = FindHandler( w );
                    if( handler == null )
                    {
                        response.ResponseType = -1;
                        response.Payload = new InvalidOperationException( "Handler not found for command type " + w.CommandServerType.FullName );
                    }
                    else response.ResponseType = 0;

                    request.CommandResponsePromise.SetResult( response );

                    DoHandle( async () =>
                    {
                        var deferredResponse = new CommandResponse();
                        deferredResponse.CommandId = w.CommandId;
                        try
                        {
                            deferredResponse.Payload = await handler.HandleAsync( w.CommandBody );
                            deferredResponse.ResponseType = 0;
                        }
                        catch( Exception ex )
                        {
                            deferredResponse.ResponseType = -2;
                            deferredResponse.Payload = ex;
                        }
                        EventChannel.Instance.Dispatch( w.CallbackId, deferredResponse );
                    } );

                    Thread.Sleep( 100 );
                }
            } );
        }

        private void DoHandle( Func<Task> lambda )
        {
            Task.Run( lambda );
        }

        private ICommandHandler FindHandler( CommandWrapper w )
        {
            ICommandHandler handler = null;
            HandlerMap.TryGetValue( w.CommandServerType, out handler );
            return handler;
        }

        public void RegisterHandler<T>( ICommandHandler handler )
        {
            if( handler == null )
                throw new ArgumentNullException( nameof( handler ) );
            if( HandlerMap.ContainsKey( typeof( T ) ) )
                throw new ArgumentException( "An handler is already registered for this command " + typeof( T ).FullName );

            HandlerMap.Add( typeof( T ), handler );
        }

        public Task<IResponse> ProcessCommandAsync( CommandRequest command )
        {
            if( command == null )
                throw new ArgumentNullException( nameof( command ) );

            TaskCompletionSource<IResponse> future = new TaskCompletionSource<IResponse>();

            IncomingRequests.Add( new PendingRequest
            {
                CommandRequest = command,
                CommandResponsePromise = future
            } );

            return future.Task;
        }
        public void Dispose()
        {
            _disposed = true;
            IncomingRequests.Dispose();
        }
    }

    public static class Servers
    {
        static Dictionary<string, ICommandServer> ServerDns { get; } = new Dictionary<string, ICommandServer>();
        public static ICommandServer GetServer( string serverAddress )
        {
            ICommandServer serv;
            if( !ServerDns.TryGetValue( serverAddress, out serv ) )
            {
                throw new InvalidOperationException( "Server not found or not running !" );
            }
            return serv;
        }

        internal static void Register( string serverAddress, ICommandServer server )
        {
            ServerDns.Add( serverAddress, server );
        }
    }

    class CommandWrapper
    {
        public string CallbackId { get; set; }
        public object CommandBody { get; set; }
        public Guid CommandId { get; set; }
        public Type CommandServerType { get; set; }
    }

    #endregion
}
