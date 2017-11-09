using CK.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
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

        public string Name => "DefaultReceiver";

        public bool AcceptCommand( ICommandContext context )
        {
            return true;
        }

        public async Task<Response> ReceiveCommand<T>( T command, ICommandContext context )
        {
            Response response = null;
            try
            {
                var result = await _invoker.Invoke( command, context );
                response = new Response<object>( context.CommandId )
                {
                    Payload = result
                };
            }
            catch( Exception ex )
            {
                response = new ErrorResponse( ex, context.CommandId );
            }
            Debug.Assert( response != null );
            return response;
        }
    }
}
