﻿//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using CK.Core;
//using CK.Crs;
//using CK.Crs.Tests.Handlers;
//using NUnit.Framework;
//using CK.Crs.Runtime;
//using Xunit;
//using Newtonsoft.Json;

//namespace CK.Crs.Tests
//{
//    public class WithClientApiTest
//    {
//        [Test][Test]
//        public async Task SendCommandAndWaitForEvents()
//        {
//            string serverAddress = "http://MyDumbServer/c/";
//            var serviceProvider = TestHelper.CreateServiceProvider( Util.ActionVoid );
//            var Testories = new DefaultTestories( serviceProvider );
//            using( var server = new CommandReceiverHost( serverAddress, new CommandReceiver( new BasicExecutionStrategySelector( Testories ), Testories ) ) )
//            {
//                // Server initialization
//                server.Run();

//                // Client Code
//                var sender = new ClientCommandSender();

//                TransferAmountCommand command = new TransferAmountCommand
//                {
//                    DestinationAccountId = Guid.NewGuid(),
//                    SourceAccountId = Guid.NewGuid(),
//                    Amount = 400
//                };

//                var route = new RoutedCommandDescriptor(
//                    new CommandRoutePath( "/c", "TransferAmountCommand" ),
//                    new CommandDescription
//                    {
//                        Name = "TransferAmountCommand",
//                        CommandType = typeof( TransferAmountCommand ),
//                        HandlerType = typeof( TransferAlwaysSuccessHandler )
//                    });
//                ClientCommandResult result = await sender.SendAsync(serverAddress, command, route );
//                await result.OnResultAsync<TransferAmountCommand.Result>( @event =>
//                {
//                    Console.WriteLine( @event.CancellableDate.ToString() );
//                    Console.WriteLine( @event.EffectiveDate.ToString() );
//                    return Task.FromResult( 0 );
//                } );

//                await result.OnErrorAsync( ex =>
//                {
//                    Console.WriteLine( ex.ToString() );
//                    return Task.FromResult( 0 );
//                } );

//                WithdrawMoneyCommand withDrawCommand = new WithdrawMoneyCommand
//                {
//                    AccountId = Guid.NewGuid(),
//                    Amount = 20
//                };

//                AutoResetEvent e = new AutoResetEvent(false);

//                var route2 = new RoutedCommandDescriptor(
//                    new CommandRoutePath( "/c", "WithdrawMoneyCommand" ),
//                    new CommandDescription
//                    {
//                        CommandType = typeof( WithdrawMoneyCommand ),
//                        HandlerType = typeof( WithDrawyMoneyHandler )
//                    });
//                result = await sender.SendAsync( serverAddress, withDrawCommand, route2 );
//                await result.OnResultAsync<WithdrawMoneyCommand.Result>( @event =>
//                {
//                    Console.WriteLine( @event.Success.ToString() );
//                    e.Set();
//                    return Task.FromResult( 0 );
//                } );

//                await result.OnErrorAsync( ex =>
//                {
//                    Console.WriteLine( ex.ToString() );
//                    e.Set();
//                    return Task.FromResult( 0 );
//                } );

//                e.WaitOne( TimeSpan.FromSeconds( 2 ) );
//            }
//        }
//    }

//    #region Client SDK

//    class ClientCommandSender
//    {
//        public async Task<ClientCommandResult> SendAsync<T>( string address, T command, RoutedCommandDescriptor commandDescriptor )
//        {
//            CommandRequest request = new CommandRequest( address, JsonConvert.SerializeObject( command ), user );
//            ClientCommandResult result = null;
//            try
//            {
//                CommandResponse response = await CommandChannel.SendAsync( address, request );
//                if( response.ResponseType == CommandResponseType.Deferred )
//                {
//                    result = new DeferredResult();
//                    result.Result = request.CallbackId;
//                }
//                else if( response.ResponseType == CommandResponseType.Direct )
//                {
//                    result = new DirectResult();
//                    result.Result = response.Payload;
//                }
//                else throw new InvalidOperationException();
//                result.CommandId = response.CommandId;
//                result.IsSuccess = response.ResponseType >= 0;
//            }
//            catch( Exception ex )
//            {
//                result = new DirectResult();
//                result.IsSuccess = false;
//            }
//            return result;
//        }
//    }

//    public abstract class ClientCommandResult
//    {
//        public Guid CommandId { get; set; }

//        public bool IsFailure { get; set; }

//        public bool IsSuccess { get; set; }

//        public object Result { get; set; }

//        public abstract Task OnResultAsync<TEvent>( Func<TEvent, Task> callback ) where TEvent : class;

//        public abstract Task OnErrorAsync( Func<Exception, Task> exCallback );
//    }

//    public class DirectResult : ClientCommandResult
//    {
//        public override async Task OnResultAsync<TEvent>( Func<TEvent, Task> callback )
//        {
//            TEvent evt = Result as TEvent;
//            if( evt != null ) await callback( evt );
//        }
//        public override async Task OnErrorAsync( Func<Exception, Task> exCallback )
//        {
//            Exception ex = Result as Exception;
//            if( ex != null ) await exCallback( ex );
//        }
//    }

//    public class DeferredResult : ClientCommandResult
//    {
//        public override Task OnResultAsync<TEvent>( Func<TEvent, Task> callback )
//        {
//            return EventChannel.Instance.ListenAsync( CommandId, (string)Result, callback );
//        }

//        public override Task OnErrorAsync( Func<Exception, Task> exCallback )
//        {
//            return EventChannel.Instance.ListenAsync( CommandId, (string)Result, exCallback );
//        }
//    }

//    #endregion

//    #region Commands and Events Channels

//    public static class CommandChannel
//    {
//        internal static Task<CommandResponse> SendAsync( string serverAddress, CommandRequest request )
//        {
//            return Servers.GetCommandReceiverHost( serverAddress ).ProcessRequestAsync( request );
//        }
//    }

//    public struct PendingCommandInfo : IEquatable<PendingCommandInfo>
//    {
//        public Guid CommandId;

//        public string CallbackId;

//        public PendingCommandInfo( Guid commandId, string callbackId )
//        {
//            CommandId = commandId;
//            CallbackId = callbackId;
//        }

//        public bool Equals( PendingCommandInfo other )
//        {
//            return other.CallbackId.Equals( CallbackId ) && other.CommandId.Equals( CommandId );
//        }

//        public override int GetHashCode()
//        {
//            return CommandId.GetHashCode() ^ CallbackId.GetHashCode();
//        }

//        public override bool Equals( object obj )
//        {
//            return Equals( (PendingCommandInfo)obj );
//        }
//    }


//    public class EventChannel : ICommandResponseDispatcher
//    {
//        public static EventChannel Instance = new EventChannel();

//        /// <summary>
//        /// The unique connection identifier for this instance
//        /// </summary>
//        public string ConnectionId { get; } = Guid.NewGuid().ToString( "N" );

//        /// <summary>
//        /// Stop listen to events after the configured timeout value
//        /// </summary>
//        public TimeSpan DefaultTimeout { get; } = TimeSpan.FromMinutes( 1 );

//        /// <summary>
//        /// A list of pending events linked to this connection
//        /// </summary>
//        public ConcurrentDictionary<PendingCommandInfo, object> PendingEvents { get; } = new ConcurrentDictionary<PendingCommandInfo, object>();

//        internal Task ListenAsync<TEvent>( Guid commandId, string callbackId, Func<TEvent, Task> callback ) where TEvent : class
//        {
//            return Task.Testory.StartNew( async () =>
//            {
//                bool shouldContinue = true;
//                DateTime stopReceiving = DateTime.UtcNow.Add(DefaultTimeout);
//                PendingCommandInfo key = new PendingCommandInfo( commandId, callbackId );

//                // Message pump
//                while( DateTime.UtcNow < stopReceiving && shouldContinue )
//                {
//                    object v;
//                    if( PendingEvents.TryGetValue( key, out v ) )
//                    {
//                        if( v != null && v is TEvent )
//                        {
//                            if( PendingEvents.TryRemove( key, out v ) )
//                            {
//                                try
//                                {
//                                    await callback( v as TEvent );
//                                }
//                                finally
//                                {
//                                    shouldContinue = false;
//                                }
//                            }
//                        }
//                    }
//                    await Task.Delay( 200 );
//                }
//            } );
//        }

//        public Task DispatchAsync( string callbackId, CommandResponse response, CancellationToken cancellationToken = default( CancellationToken ) )
//        {
//            if( String.IsNullOrEmpty( callbackId ) ) throw new ArgumentNullException( nameof( callbackId ) );
//            if( response == null ) throw new ArgumentNullException( nameof( response ) );

//            var key = new PendingCommandInfo( response.CommandId, callbackId);
//            PendingEvents.TryAdd( key, response.Payload );

//            return Task.FromResult( 0 );
//        }
//    }
//    #endregion

//    #region Command Receiver Host

//    class CommandReceiverHost : IDisposable
//    {
//        bool _disposed = false;
//        ICommandReceiver _commandReceiver;
//        BlockingCollection<PendingRequest> IncomingRequests { get; } = new BlockingCollection<PendingRequest>();

//        public CommandReceiverHost( string serverAddress, ICommandReceiver commandReceiver )
//        {
//            Servers.Register( serverAddress, this );
//            _commandReceiver = commandReceiver;
//        }

//        internal void Run()
//        {
//            Task.Testory.StartNew( async () =>
//            {
//                while( !_disposed )
//                {
//                    PendingRequest pending = IncomingRequests.Take();
//                    if( pending == null ) continue;

//                    CommandResponse response = await _commandReceiver.ProcessCommandAsync( pending.CommandRequest );
//                    pending.CommandResponsePromise.SetResult( response );
//                    Thread.Sleep( 100 );
//                }
//            } );
//        }

//        class PendingRequest
//        {
//            public CommandRequest CommandRequest { get; set; }

//            public TaskCompletionSource<CommandResponse> CommandResponsePromise { get; set; }
//        }

//        public Task<CommandResponse> ProcessRequestAsync( CommandRequest command )
//        {
//            if( command == null ) throw new ArgumentNullException( nameof( command ) );

//            TaskCompletionSource<CommandResponse> future = new TaskCompletionSource<CommandResponse>();

//            IncomingRequests.Add( new PendingRequest
//            {
//                CommandRequest = command,
//                CommandResponsePromise = future
//            } );

//            return future.Task;
//        }

//        public void Dispose()
//        {
//            _disposed = true;
//            IncomingRequests.Dispose();
//        }
//    }

//    static class Servers
//    {
//        static Dictionary<string, CommandReceiverHost> ServerDns { get; } = new Dictionary<string, CommandReceiverHost>();
//        public static CommandReceiverHost GetCommandReceiverHost( string serverAddress )
//        {
//            CommandReceiverHost serv;
//            if( !ServerDns.TryGetValue( serverAddress, out serv ) )
//            {
//                throw new InvalidOperationException( "Server not found or not running !" );
//            }
//            return serv;
//        }

//        internal static void Register( string serverAddress, CommandReceiverHost server )
//        {
//            ServerDns.Add( serverAddress, server );
//        }
//    }
//    #endregion
//}
