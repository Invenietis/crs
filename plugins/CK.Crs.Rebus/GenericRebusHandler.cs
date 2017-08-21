using CK.Core;
using Rebus.Handlers;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Rebus
{
    class GenericRebusHandler<T> : IHandleMessages<T>
    {
        ICommandHandlerInvoker _invoker;
        ICommandRegistry _registry;
        global::Rebus.Bus.IBus _bus;
        IResultStrategy _resultStrategy;

        public GenericRebusHandler( ICommandHandlerInvoker invoker, ICommandRegistry registry, IResultStrategy resultStrategy, global::Rebus.Bus.IBus bus )
        {
            _invoker = invoker;
            _registry = registry;
            _resultStrategy = resultStrategy;
            _bus = bus;
        }

        public async Task Handle( T command )
        {
            var model = _registry.Registration.FirstOrDefault( r => r.CommandType == typeof( T ) );
            if( model != null )
            {
                var msgContext = MessageContext.Current;
                using( var token = new CancellationTokenSource() )
                {
                    var context = new CommandContext(
                        msgContext.GetCommandId(),
                        msgContext.GetActivityMonitor(),
                        model,
                        msgContext.GetCallerId(),
                        token.Token );

                    var resultReceiver = _resultStrategy.GetResultReceiver( model );
                    if( resultReceiver  != null )
                    {
                        await resultReceiver.ReceiveResult( command, context );
                        return;
                    }

                    if( model.HandlerType == null )
                        throw new InvalidOperationException( $"There is no handler configured for the command {model.CommandType}" );

                    context.Monitor.Trace( "Registering cancellations" );

                    token.Token.Register( () =>
                    {
                        context.Monitor.Trace( "Cancellation called. Aborting transactions." );

                        msgContext.TransactionContext.Abort();
                        msgContext.AbortDispatch();
                    } );

                    context.Monitor.Info( $"Executing handler {model.HandlerType} for {model.CommandType}" );

                    var result = await _invoker.Invoke( command, context );
                    if( result != null )
                    {
                        await _bus.Reply( result, new Dictionary<string, string>
                        {
                            { nameof( ICommandContext.Monitor ), context.Monitor.DependentActivity().CreateToken().ToString() },
                            { nameof( ICommandContext.CommandId ), context.CommandId.ToString() },
                            { nameof( ICommandContext.CallerId ), context.CallerId }
                        } );
                    }
                }
            }
        }
    }
}
