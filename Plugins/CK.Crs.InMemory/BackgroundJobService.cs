using CK.Core;
using CK.Crs.Results;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.InMemory
{
    public class BackgroundJobService : Hosting.ICommandHost
    {
        private Task _proxy;
        private CommandJobQueue _commandJobQueue;
        private CancellationTokenSource _cancellationToken;

        public bool Initialized { get; private set; }

        public virtual void Start( IActivityMonitor monitor, IServiceProvider services, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            _commandJobQueue = services.GetRequiredService<CommandJobQueue>();
            _cancellationToken = new CancellationTokenSource();
            _proxy = new Task( async () =>
            {
                monitor.Trace( "CommandJobQueue running..." );
                while( _commandJobQueue.CanGetNextJob && !_cancellationToken.IsCancellationRequested )
                {
                    monitor.Trace( "Waiting for the next command to take..." );
                    var job = _commandJobQueue.GetNextJob( _cancellationToken.Token );
                    if( job != null )
                    {
                        using( monitor.OpenTrace( $" Command {job.CommandContext.Model.Name} - {job.CommandContext.CommandId} taken" ) )
                        {
                            try
                            {
                                using( var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope() )
                                {
                                    using( job.CommandContext.ChangeMonitor() )
                                    {
                                        ITypedCommandHandlerInvoker invoker = scope.ServiceProvider.GetRequiredService<ITypedCommandHandlerInvoker>();
                                        IResultReceiverProvider resultStrategy = scope.ServiceProvider.GetRequiredService<IResultReceiverProvider>();
                                        await HandleCommandJobAsync( monitor, job, invoker, resultStrategy );
                                    }
                                }
                            }
                            catch( Exception ex )
                            {
                                monitor.Error( ex );
                            }
                        }
                    }
                }
                monitor.Trace( "CommandJobQueue stoping..." );
            } );
            _proxy.Start();
        }

        protected virtual async Task HandleCommandJobAsync( IActivityMonitor monitor, ICommandJob job, ITypedCommandHandlerInvoker invoker, IResultReceiverProvider resultStrategy )
        {
            monitor.Trace( "Invoking the command..." );

            var resultReceiver = resultStrategy.GetResultReceiver( job.CommandContext );
            if( resultReceiver == null ) monitor.Warn( "No result receiver available for this command. Unable to communicate the result..." );

            try
            {
                object result = await invoker.Invoke( job.Command, job.CommandContext ).ConfigureAwait( false );
                if( resultReceiver != null )
                {
                    monitor.Trace( "Sending the result." );
                    await resultReceiver.InvokeGenericResult( result, job.CommandContext ).ConfigureAwait( false );
                }
            }
            catch( Exception ex )
            {
                monitor.Trace( "Error during command invokation." );
                if( resultReceiver != null )
                {
                    monitor.Trace( "Sending the result." );
                    await resultReceiver.ReceiveError( ex, job.CommandContext ).ConfigureAwait( false );
                }
            }
        }

        public virtual void Stop( IActivityMonitor monitor, IServiceProvider services, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            _cancellationToken?.Cancel();
            _cancellationToken?.Dispose();
            _proxy?.Dispose();
        }
    }
}
