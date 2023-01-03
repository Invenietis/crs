using System;
using System.Collections.Concurrent;
using System.Threading;

namespace CK.Crs.InMemory
{
    class CommandJobQueue : IDisposable
    {
        readonly BlockingCollection<CommandJob> _queue;

        public CommandJobQueue()
        {
            _queue = new BlockingCollection<CommandJob>();
        }

        public void Dispose()
        {
            IsDisposed = true;
            _queue.CompleteAdding();
            _queue.Dispose();
        }

        public bool IsDisposed { get; private set; }

        public bool CanGetNextJob => !IsDisposed && !_queue.IsCompleted;

        public CommandJob GetNextJob( CancellationToken token = default( CancellationToken) )
        {
            if( IsDisposed ) return null;
            try
            {
                return _queue.Take( token );
            }
            catch( OperationCanceledException )
            {
                return null;
            }
        }

        public bool Push( object comand, ICommandContext context )
        {
            if( !IsDisposed && !_queue.IsAddingCompleted )
            {
                _queue.Add( new CommandJob
                {
                    Command = comand,
                    CommandContext = new CommandJobContext( context )
                } );
                return true;
            }
            return false;
        }
    }
}
