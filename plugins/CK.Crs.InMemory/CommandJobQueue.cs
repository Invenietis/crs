using CK.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.InMemory
{
    class CommandJobQueue : IDisposable
    {
        readonly BlockingCollection<CommandJob> _queue;
        public CommandJobQueue( ICommandHandlerInvoker invoker, IResultReceiverProvider resultStrategy )
        {
            _queue = new BlockingCollection<CommandJob>();
            Task.Run( async () =>
            {
                var monitor = new ActivityMonitor();
                monitor.Trace( "CommandJobQueue running..." );
                while( !_queue.IsCompleted )
                {
                    monitor.Trace( "Waiting for the next command to take..." );
                    var job = _queue.Take();
                    using( monitor.OpenTrace( $" Command {job.CommandContext.Model.Name} - {job.CommandContext.CommandId} taken" ) )
                    {
                        try
                        {
                            using( job.Token != null ? monitor.StartDependentActivity( job.Token ) : Util.EmptyDisposable )
                            {
                                var context2 = new CommandContext( job.CommandContext.CommandId, monitor, job.CommandContext.Model, job.CommandContext.CallerId, default( CancellationToken ) );
                                monitor.Trace( "Invoking the command..." );
                                var deferredResult = await invoker.Invoke( job.Command, context2 );

                                var resultReceiver = resultStrategy.GetResultReceiver( context2 );
                                if( resultReceiver == null ) monitor.Warn( "No result receiver available for this command. Unable to communicate the result..." );
                                else
                                {
                                    monitor.Trace( "Sending the result." );
                                    await resultReceiver.ReceiveResult( deferredResult, context2 );
                                }
                            }
                        }
                        catch( Exception ex )
                        {
                            monitor.Error( ex );
                        }
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
            _queue.Add( new CommandJob
            {
                Command = comand,
                CommandContext = context,
                Token = depToken
            } );
        }
    }
}
