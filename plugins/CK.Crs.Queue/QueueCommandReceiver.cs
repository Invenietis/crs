using CK.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Queue
{
    class QueueCommandReceiver : ICommandReceiver
    {
        private readonly InMemoryQueue _inMemoryQueue;

        public QueueCommandReceiver( InMemoryQueue inMemoryQueue )
        {
            _inMemoryQueue = inMemoryQueue;
        }

        public Task<Response> ReceiveCommand<T>( T command, ICommandContext context ) where T : class
        {
            if( context.Model.HasInProcessQueueTag() )
            {
                _inMemoryQueue.Push( command, context );
                return Task.FromResult<Response>( new DeferredResponse( context.CommandId, context.CallerId ) );
            }
            return Task.FromResult<Response>( null );
        }
    }
}
