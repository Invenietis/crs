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
        public CommandJobQueue( ITypedCommandHandlerInvoker invoker, IResultReceiverProvider resultStrategy )
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
                            using( job.CommandContext.ChangeMonitor() )
                            {
                                monitor.Trace( "Invoking the command..." );

                                var resultReceiver = resultStrategy.GetResultReceiver( job.CommandContext );
                                if( resultReceiver == null ) monitor.Warn( "No result receiver available for this command. Unable to communicate the result..." );

                                try
                                {
                                    object result = await invoker.Invoke( job.Command, job.CommandContext ).ConfigureAwait( false );
                                    //var result = await invoker.Invoke( job.Command, job.CommandContext );
                                    if( resultReceiver != null )
                                    {
                                        monitor.Trace( "Sending the result." );
                                        await resultReceiver.InvokeGenericResult( result, job.CommandContext ).ConfigureAwait( false );
                                        //await resultReceiver.ReceiveResult( result, job.CommandContext );
                                    }
                                }
                                catch( Exception ex )
                                {
                                    monitor.Trace( "Error during command invokation." );
                                    if( resultReceiver != null )
                                    {
                                        monitor.Trace( "Sending the result." );
                                        await resultReceiver.ReceiveError( ex, job.CommandContext );
                                    }
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
            _queue.Add( new CommandJob
            {
                Command = comand,
                CommandContext = new CommandJobContext( context )
            } );
        }
    }
}
