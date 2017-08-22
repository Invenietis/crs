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

        public Task<Response> ReceiveCommand<T>( T command, ICommandContext context ) where T : class
        {
            if( context.Model.HasFireAndForgetTag() )
            {
                _queue.Push( command, context );
                return Task.FromResult<Response>( new DeferredResponse( context.CommandId, context.CallerId ) );
            }
            return Task.FromResult<Response>( null );
        }
    }
}
