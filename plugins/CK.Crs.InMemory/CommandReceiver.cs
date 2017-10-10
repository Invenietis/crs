using CK.Core;
using System;
using System.Threading;
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

        public bool AcceptCommand( ICommandContext context )
        {
            bool res = context.Model.HasFireAndForgetTag();
            if( res == false )
                context.Monitor.Trace( "Command does not have the FireAndForget trait." );

            return res;
        }

        public Task<Response> ReceiveCommand<T>( T command, ICommandContext context )
        {
            context.Monitor.Trace( "Enqueuinq the command for asynchronous processing." );
            _queue.Push( command, context );
            return Task.FromResult<Response>( new DeferredResponse( context.CommandId, context.CallerId ) );
        }
    }
}
