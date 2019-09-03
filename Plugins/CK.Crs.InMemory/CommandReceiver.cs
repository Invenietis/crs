using CK.Core;
using CK.Crs.Responses;
using System.Threading.Tasks;

namespace CK.Crs.InMemory
{
    class CommandReceiver : ICommandReceiver
    {
        private readonly CommandJobQueue _queue;

        public CommandReceiver( CommandJobQueue queue )
        {
            _queue = queue;
        }

        public string Name => "InMemoryReceiver";

        public bool AcceptCommand( object command, ICommandContext context )
        {
            bool res = context.Model.HasFireAndForgetTag();
            if( res == false )
                context.Monitor.Trace( "Command does not have the FireAndForget trait." );

            return res;
        }

        public Task<Response> ReceiveCommand( object command, ICommandContext context )
        {
            context.Monitor.Trace( "Enqueuinq the command for asynchronous processing." );
            bool result = _queue.Push( command, context );
            if( result ) return Task.FromResult<Response>( new DeferredResponse( context.CommandId, context.CallerId ) );

            return Task.FromResult<Response>( new ErrorResponse( "Queue not available", context.CommandId ) );
        }
    }
}
