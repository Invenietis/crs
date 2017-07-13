using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class DefaultBus : IBus
    {
        readonly IRequestRegistry _registry;
        readonly IRequestHandlerFactory _factory;

        public DefaultBus( IRequestRegistry registry, IRequestHandlerFactory factory )
        {
            _registry = registry;
            _factory = factory;
        }

        public Task PostAsync<T>( T command, ICommandContext context ) where T : class
        {
            return SendAsync( command, context );
        }

        public Task<object> SendAsync<T>( T command, ICommandContext context ) where T : class
        {
            var desc = _registry.Registration.SingleOrDefault( c => c.Type == typeof( T ) );
            return DoSendAsync( command, context, desc );
        }

        public async Task PublishAsync<T>( T evt, ICommandContext context ) where T : class
        {
            var all = _registry.Registration.Where( c => c.Type == typeof( T ) );
            foreach( var desc in all ) await DoSendAsync( evt, context, desc );
        }

        private async Task<object> DoSendAsync<T>( T command, ICommandContext context, RequestDescription desc ) where T : class
        {
            if( desc == null ) throw new ArgumentException( String.Format( "Command {0} not registered", typeof( T ).Name ) );

            dynamic handler = _factory.CreateHandler( desc.HandlerType );
            if( handler == null ) throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to created", desc.HandlerType ) );
            try
            {
                if( handler is ICommandHandlerWithResult )
                {
                    var res = await handler.HandleAsync( command, context );
                    return res;
                }
                if( handler is ICommandHandler )
                {
                    await handler.HandleAsync( command, context );
                    return null;
                }
                if( handler is IEventHandler )
                {
                    await handler.HandleAsync( command, context );
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
