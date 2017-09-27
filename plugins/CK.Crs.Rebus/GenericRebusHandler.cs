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
        IResultReceiverProvider _resultStrategy;

        public GenericRebusHandler( ICommandHandlerInvoker invoker, ICommandRegistry registry, IResultReceiverProvider resultStrategy, global::Rebus.Bus.IBus bus )
        {
            _invoker = invoker;
            _registry = registry;
            _resultStrategy = resultStrategy;
            _bus = bus;
        }

        public async Task Handle( T command )
        {
            var msgContext = MessageContext.Current;

            CommandName commandName = msgContext.GetCommandName();
            CommandModel model = _registry.GetCommandByName( commandName );
            if( model != null )
            {
                using( var token = new CancellationTokenSource() )
                {
                    var context = new CommandContext(
                        msgContext.GetCommandId(),
                        msgContext.GetActivityMonitor(),
                        model,
                        msgContext.GetCallerId(),
                        token.Token );

                    if( model.ResultType == typeof( T ) )
                    {
                        context.Monitor.Trace( "Receiving result on the Rebus reply bus" );

                        var resultReceiver = _resultStrategy.GetResultReceiver( context );
                        if( resultReceiver != null )
                        {
                            await resultReceiver.ReceiveResult( command, context );
                        }
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
                        var resultHeaders = context.CreateResultHeaders();
                        await _bus.Reply( result, resultHeaders );
                    }
                }
            }
        }
    }
}
