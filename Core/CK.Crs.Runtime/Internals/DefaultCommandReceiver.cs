using CK.Core;
using CK.Crs.Responses;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    internal class DefaultCommandReceiver : ICommandReceiver
    {
        readonly ITypedCommandHandlerInvoker _invoker;

        public DefaultCommandReceiver(ITypedCommandHandlerInvoker invoker )
        {
            _invoker = invoker;
        }

        public string Name => "DefaultReceiver";

        public bool AcceptCommand( ICommandContext context )
        {
            return true;
        }

        public async Task<Response> ReceiveCommand( object command, ICommandContext context )
        {
            Response response = null;
            try
            {
                var result = await _invoker.Invoke( command, context );
                response = ResponseUtil.CreateGenericResponse( result, context );
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
