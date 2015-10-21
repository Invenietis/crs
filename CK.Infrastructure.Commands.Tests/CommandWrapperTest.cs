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
    [AsyncCommand]
    public class TransferAmountCommand
    {
        public Guid SourceAccountId { get; set; }

        public Guid DestinationAccountId { get; set; }

        public decimal Amount { get; set; }
    }

    public class WithdrawMoneyCommand
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }

    public class AmountTransferredEvent
    {
        public DateTime EffectiveDate { get; set; }

        public DateTime CancellableDate { get; set; }
    }
    public class MoneyWithdrawedEvent
    {
        public bool Success { get; set; }
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

    public class WithDrawyMoneyHandler : CommandHandler<WithdrawMoneyCommand>
    {
        public override Task<object> DoHandleAsync( WithdrawMoneyCommand command )
        {
            var result =  new MoneyWithdrawedEvent
            {
                Success = true
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
            using( var server = new DumbCommandReceiver( serverAddress ) )
            {
                // Server initialization
                server.RegisterHandler<TransferAmountCommand, TransferAlwaysSuccessHandler>();
                server.RegisterHandler<WithdrawMoneyCommand, WithDrawyMoneyHandler>();
                server.Run();

                // Client Code
                var sender = new ClientCommandSender();

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

                WithdrawMoneyCommand withDrawCommand = new WithdrawMoneyCommand
                {
                    AccountId = Guid.NewGuid(),
                    Amount = 20
                };

                result = await sender.SendAsync(serverAddress, withDrawCommand );
                await result.OnResultAsync<MoneyWithdrawedEvent>( @event =>
                {
                    Output.WriteLine( @event.Success.ToString() );
                    return Task.FromResult( 0 );
                } );

                await result.OnErrorAsync( ex =>
                {
                    Console.WriteLine( ex.ToString() );
                    return Task.FromResult( 0 );
                } );

                await Task.Delay( TimeSpan.FromSeconds( 60 ) );
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
                ICommandResponse response = await CommandChannel.SendAsync( address, request );
                if( response.ResponseType == CommandResponseType.Deferred )
                {
                    result = new DeferredResult();
                    result.Result = request.CallbackId;
                }
                else if( response.ResponseType == CommandResponseType.Direct )
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

    public interface ICommandRequest
    {
        object Command { get; }

        string CallbackId { get; }

        Type CommandServerType { get; }
    }

    public enum CommandResponseType
    {
        Error = -1,
        Direct = 0,
        Deferred = 1
    }

    public interface ICommandResponse
    {
        Guid CommandId { get; }

        CommandResponseType ResponseType { get; set; }

        object Payload { get; set; }
    }

    public interface ICommandHandlerRegistry
    {
        void RegisterHandler<T, THandler>();
    }

    /// <summary>
    /// Defines the contract a command receiver should implement
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// Process a <see cref="ICommandRequest"/> and returns a <see cref="ICommandResponse"/>
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<ICommandResponse> ProcessCommandAsync( ICommandRequest command );
    }

    public interface ICommandContext
    {
        ICommandRequest Request { get; }

        ICommandResponse Response { get; }

        Type HandlerType { get; }

        bool IsAsynchronous { get; }
    }

    public class CommandRequest : ICommandRequest
    {
        public CommandRequest( object command )
        {
            Command = command;
        }

        public object Command { get; set; }

        public string CallbackId { get; set; }
        public Type CommandServerType { get; set; }
    }

    public class CommandResponse : ICommandResponse
    {
        public Guid CommandId { get; set; }

        public CommandResponseType ResponseType { get; set; }

        public object Payload { get; set; }
    }
    public class CommandContext : ICommandContext
    {
        public ICommandRequest Request { get; set; }

        public ICommandResponse Response { get; set; }

        public Type HandlerType { get; set; }

        public bool IsAsynchronous { get; set; }
    }

    public abstract class CommandHandler<T> : ICommandHandler where T : class
    {
        public Task<object> HandleAsync( object command )
        {
            return DoHandleAsync( command as T );
        }

        public abstract Task<object> DoHandleAsync( T command );
    }

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
    public class AsyncCommandAttribute : Attribute
    {
    }

    #endregion

    #region Commands and Events Channels

    public static class CommandChannel
    {
        internal static Task<ICommandResponse> SendAsync( string serverAddress, ICommandRequest request )
        {
            return Servers.GetCommandReceiver( serverAddress ).ProcessCommandAsync( request );
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

        public void Dispatch( string callbackId, ICommandResponse response )
        {
            if( String.IsNullOrEmpty( callbackId ) ) throw new ArgumentNullException( nameof( callbackId ) );
            if( response == null ) throw new ArgumentNullException( nameof( response ) );

            var key = new PendingCommandInfo( response.CommandId, callbackId);
            PendingEvents.TryAdd( key, response.Payload );
        }
    }
    #endregion

    #region Server Command Receiver

    class DumbCommandReceiver : ICommandReceiver, ICommandHandlerRegistry, IDisposable
    {
        bool _disposed = false;
        public DumbCommandReceiver( string serverAddress )
        {
            Servers.Register( serverAddress, this );
        }

        class PendingRequest
        {
            public ICommandRequest CommandRequest { get; set; }

            public TaskCompletionSource<ICommandResponse> CommandResponsePromise { get; set; }
        }

        BlockingCollection<PendingRequest> IncomingRequests { get; } = new BlockingCollection<PendingRequest>();

        public Task<ICommandResponse> ProcessCommandAsync( ICommandRequest command )
        {
            if( command == null )
                throw new ArgumentNullException( nameof( command ) );

            TaskCompletionSource<ICommandResponse> future = new TaskCompletionSource<ICommandResponse>();

            IncomingRequests.Add( new PendingRequest
            {
                CommandRequest = command,
                CommandResponsePromise = future
            } );

            return future.Task;
        }
        internal void Run()
        {
            Task.Factory.StartNew( async () =>
            {
                while( !_disposed )
                {
                    PendingRequest pending = IncomingRequests.Take();
                    if( pending == null ) continue;

                    var context = new CommandContext();
                    context.Request = pending.CommandRequest;
                    context.Response = new CommandResponse
                    {
                        CommandId = Guid.NewGuid()
                    };

                    if( !HandlerMap.ContainsKey( pending.CommandRequest.CommandServerType ) )
                    {
                        context.Response.ResponseType = CommandResponseType.Error;
                        context.Response.Payload = new InvalidOperationException( "Handler not found for command type " + context.Request.CommandServerType );
                        pending.CommandResponsePromise.SetResult( context.Response );
                    }
                    else
                    {
                        context.IsAsynchronous = ShouldHandleAsynchronously( context.Request.CommandServerType );
                        context.HandlerType = HandlerMap[context.Request.CommandServerType];
                        if( context.IsAsynchronous )
                        {
                            context.Response.ResponseType = CommandResponseType.Deferred; // Deferred
                            pending.CommandResponsePromise.SetResult( context.Response );

                            new Task( async ( state ) => await Process( state as ICommandContext ), context ).Start( TaskScheduler.Current );
                        }
                        else
                        {
                            await Process( context );
                            context.Response.ResponseType = CommandResponseType.Direct; // Direct
                            pending.CommandResponsePromise.SetResult( context.Response );
                        }
                    }


                    Thread.Sleep( 100 );
                }
            } );
        }

        public async Task Process( ICommandContext ctx )
        {
            try
            {
                ICommandHandler handler = Activator.CreateInstance( ctx.HandlerType ) as ICommandHandler;
                ctx.Response.Payload = await handler.HandleAsync( ctx.Request.Command );
                ctx.Response.ResponseType = CommandResponseType.Direct; // Event
            }
            catch( Exception ex )
            {
                ctx.Response.ResponseType = CommandResponseType.Error;
                ctx.Response.Payload = ex;
            }
            if( ctx.IsAsynchronous )
            {
                EventChannel.Instance.Dispatch( ctx.Request.CallbackId, ctx.Response );
            }
        }

        protected virtual bool ShouldHandleAsynchronously( Type commandServerType )
        {
            // Dumpb impl
            return commandServerType.GetCustomAttributes( typeof( AsyncCommandAttribute ), false ).Length > 0;
        }

        Dictionary<Type, Type> HandlerMap { get; } = new Dictionary<Type, Type>();

        public void RegisterHandler<T, THandler>()
        {
            if( HandlerMap.ContainsKey( typeof( T ) ) )
                throw new ArgumentException( "An handler is already registered for this command " + typeof( T ).FullName );

            HandlerMap.Add( typeof( T ), typeof( THandler ) );
        }

        public void Dispose()
        {
            _disposed = true;
            IncomingRequests.Dispose();
        }
    }

    public static class Servers
    {
        static Dictionary<string, ICommandReceiver> ServerDns { get; } = new Dictionary<string, ICommandReceiver>();
        public static ICommandReceiver GetCommandReceiver( string serverAddress )
        {
            ICommandReceiver serv;
            if( !ServerDns.TryGetValue( serverAddress, out serv ) )
            {
                throw new InvalidOperationException( "Server not found or not running !" );
            }
            return serv;
        }

        internal static void Register( string serverAddress, ICommandReceiver server )
        {
            ServerDns.Add( serverAddress, server );
        }
    }
    #endregion
}
