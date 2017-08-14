using CK.Core;
using Rebus.Handlers;
using Rebus.Pipeline;
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
        IRequestHandlerFactory _factory;
        IRequestRegistry _registry;
        public GenericRebusHandler( IRequestHandlerFactory factory, IRequestRegistry registry )
        {
            _factory = factory;
            _registry = registry;
        }

        public async Task Handle( T message )
        {
            var registration = _registry.Registration.FirstOrDefault( r => r.Type == typeof( T ) );
            if( registration != null )
            {
                var msgContext = MessageContext.Current;
                using( var token = new CancellationTokenSource() )
                {
                    var context = new CommandContext(
                        msgContext.GetCommandId(),
                        typeof( T ),
                        msgContext.GetActivityMonitor(),
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
                    var result = await ExecuteRequest( message, context, registration );

                }
            }
        }

        private async Task<object> ExecuteRequest( T command, ICommandContext context, RequestDescription desc )
        {
            var handler = _factory.CreateHandler( desc.HandlerType );
            if( handler == null ) throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to created", desc.HandlerType ) );
            try
            {
                if( handler is ICommandHandlerWithResult<T> )
                {
                    throw new NotImplementedException( "WTF we do with the result ?? Should we dispatch to an auto-queue ?" );
                }
                if( handler is ICommandHandler )
                {
                    await ((ICommandHandler<T>)handler).HandleAsync( command, context );
                    return null;
                }
                if( handler is IEventHandler )
                {
                    await ((IEventHandler<T>)handler).HandleAsync( command, context );
                    return null;
                }
                throw new NotSupportedException( $"The given eventHandler {desc.HandlerType} is not supported" );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
                return null;
            }
            finally
            {
                _factory.ReleaseHandler( handler );
            }
        }
    }
}
