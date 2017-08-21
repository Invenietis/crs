using CK.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Queue
{
    class InMemoryCommandJob
    {
        public object Command { get; set; }

        public ICommandContext CommandContext { get; set; }

        public ActivityMonitor.DependentToken Token { get; set; }
    }

    class InMemoryQueue : IDisposable
    {
        BlockingCollection<InMemoryCommandJob> _queue;
        public InMemoryQueue( ICommandHandlerInvoker invoker, IResultStrategy resultStrategy )
        {
            _queue = new BlockingCollection<InMemoryCommandJob>();
            Task.Run( async () =>
            {
                var monitor = new ActivityMonitor();

                while( !_queue.IsCompleted )
                {
                    var job = _queue.Take();
                    try
                    {
                        using( monitor.StartDependentActivity( job.Token ) )
                        {
                            var context2 = new CommandContext( job.CommandContext.CommandId, monitor, job.CommandContext.Model, job.CommandContext.CallerId, default( CancellationToken ) );
                            var deferredResult = await invoker.Invoke( job.Command, context2 );
                            if( deferredResult != null )
                            {
                                var resultReceiver = resultStrategy.GetResultReceiver( context2.Model );
                                await resultReceiver.ReceiveResult( deferredResult, context2 );
                            }
                        }
                    }
                    catch( Exception ex )
                    {
                        monitor.Error( ex );
                    }
                }
            } );
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
            _queue.Dispose();
        }

        public void Push<T>( T comand, ICommandContext context )
        {
            var depToken = context.Monitor.DependentActivity().CreateToken();
            _queue.Add( new InMemoryCommandJob
            {
                Command = comand,
                CommandContext = context,
                Token = depToken
            } );
        }
    }
}
