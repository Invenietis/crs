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

        public GenericRebusHandler( ICommandHandlerInvoker invoker, ICommandRegistry registry, global::Rebus.Bus.IBus bus )
        {
            _invoker = invoker;
            _registry = registry;
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

                    context.Monitor.Trace( "Registering cancellations" );

                    token.Token.Register( () =>
                    {
                        context.Monitor.Trace( "Cancellation called. Aborting transactions." );

                        msgContext.TransactionContext.Abort();
                        msgContext.AbortDispatch();
                    } );

                    context.Monitor.Info( $"Executing handler {typeof( T )}" );
                    var result = await _invoker.Invoke( command, context, model );
                    if( result != null)
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
