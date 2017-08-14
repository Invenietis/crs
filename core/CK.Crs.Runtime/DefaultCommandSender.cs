using CK.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class DefaultCommandSender : ICommandSender
    {
        readonly IRequestRegistry _registry;
        readonly IRequestHandlerFactory _factory;

        public DefaultCommandSender( IRequestRegistry registry, IRequestHandlerFactory factory )
        {
            _registry = registry;
            _factory = factory;
        }

        public Task<object> SendAsync<T>( T command, ICommandContext context ) where T : class
        {
            var desc = _registry.Registration.SingleOrDefault( c => c.Type == typeof( T ) );
            return DoSendAsync( command, context, desc );
        }

        private async Task<object> DoSendAsync<T>( T command, ICommandContext context, RequestDescription desc ) where T : class
        {
            if( desc == null ) throw new ArgumentException( String.Format( "Command {0} not registered", typeof( T ).Name ) );

            var handler = _factory.CreateHandler( desc.HandlerType ) as ICommandHandler;
            if( handler == null ) throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to created", desc.HandlerType ) );
            try
            {
                if( handler is ICommandHandlerWithResult<T> )
                {
                    var res = await ((ICommandHandlerWithResult<T>)handler).HandleAsync( command, context );
                    return res;
                }
                if( handler is ICommandHandler<T> )
                {
                    await ((ICommandHandler<T>)handler).HandleAsync( command, context );
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
