using CK.Core;
using System;
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
            using( context.Monitor.CollectEntries( ( errors ) =>
            {
                if( errors.Count > 0 )
                    response = new ErrorResponse( string.Join( Environment.NewLine, errors.Select( e => e.ToString() ) ), context.CommandId );
            } ) )
            {
                response = new Response<object>( context.CommandId )
                {
                    Payload = await _invoker.Invoke( command, context )
                };
            }
            return response;
        }
    }
}
