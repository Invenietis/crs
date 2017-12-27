using CK.Core;
using CK.Crs.Results;
using System;
using System.Collections.Concurrent;
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
                                monitor.Trace( "Invoking the command..." );
                                var deferredResult = await invoker.Invoke( job.Command, job.CommandContext );

                                var resultReceiver = resultStrategy.GetResultReceiver( job.CommandContext );
                                if( resultReceiver == null ) monitor.Warn( "No result receiver available for this command. Unable to communicate the result..." );
                                else
                                {
                                    monitor.Trace( "Sending the result." );
                                    await resultReceiver.ReceiveResult( deferredResult, job.CommandContext );
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

        public void Push( object comand, ICommandContext context )
        {
            var depToken = context.Monitor.DependentActivity().CreateToken();
            _queue.Add( new CommandJob
            {
                Command = comand,
                CommandContext = new CommandJobContext( context ),
                Token = depToken
            } );
        }
    }
}
