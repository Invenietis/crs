using CK.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class DefaultCommandReceiver : ICommandReceiver
    {
        readonly ICommandRegistry _registry;
        readonly ICommandHandlerInvoker _invoker;

        public DefaultCommandReceiver( ICommandRegistry registry, ICommandHandlerInvoker invoker )
        {
            _registry = registry;
            _invoker = invoker;
        }

        public async Task<Response> ReceiveCommand<T>( T command, ICommandContext context ) where T : class
        {
            Response response = null;
            using( context.Monitor.CollectEntries( ( errors ) =>
            {
                if( errors.Count > 0 ) response = new ErrorResponse( string.Join( Environment.NewLine, errors.Select( e => e.ToString() ) ), context.CommandId );
            } ) )
            {
                var desc = _registry.Registration.SingleOrDefault( c => c.CommandType == typeof( T ) );
                var result = await _invoker.Invoke( command, context, desc );

                response = new Response( ResponseType.Synchronous, context.CommandId )
                {
                    Payload = result
                };

                return response;
            }
        }
    }
}
