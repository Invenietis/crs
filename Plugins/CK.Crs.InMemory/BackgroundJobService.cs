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
#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
            _proxy = new Task( async () =>
            {
                try
                {
                    monitor.Trace( $"The CRS {nameof( BackgroundJobService )} queue is running." );
                    while( _commandJobQueue.CanGetNextJob && !_cancellationToken.IsCancellationRequested )
                    {
                        monitor.Trace( "Waiting for the next command." );
                        var job = _commandJobQueue.GetNextJob( _cancellationToken.Token );
                        if( job != null )
                        {
                            using( monitor.OpenTrace( $"Processing command: {job.CommandContext.Model.Name}. CommandId: {job.CommandContext.CommandId}." ) )
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
                    monitor.Trace( $"The CRS {nameof( BackgroundJobService )} queue is stopping." );
                }
                catch( Exception ex )
                {
                    monitor.Error( $"Error in CK.Crs.BackgroundJobService", ex );
                }
            } );
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates
            _proxy.Start();
        }

        protected virtual async Task HandleCommandJobAsync( IActivityMonitor monitor, ICommandJob job, ITypedCommandHandlerInvoker invoker, IResultReceiverProvider resultStrategy )
        {
            monitor.Trace( "Invoking command." );

            var resultReceiver = resultStrategy.GetResultReceiver( job.CommandContext );
            if( resultReceiver == null ) monitor.Warn( "No result receiver available for this command. No result will be sent back." );

            try
            {
                object result = await invoker.Invoke( job.Command, job.CommandContext ).ConfigureAwait( false );
                if( resultReceiver != null )
                {
                    monitor.Trace( "Sending result." );
                    await resultReceiver.InvokeGenericResult( result, job.CommandContext ).ConfigureAwait( false );
                }
            }
            catch( Exception ex )
            {
                monitor.Trace( "Error during command invocation." );
                if( resultReceiver != null )
                {
                    monitor.Trace( "Sending result." );
                    await resultReceiver.ReceiveError( ex, job.CommandContext ).ConfigureAwait( false );
                }
            }
        }

        public virtual void Stop( IActivityMonitor monitor, IServiceProvider services, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            try
            {
                _cancellationToken?.Cancel();
                _cancellationToken?.Dispose();
                _cancellationToken = null;

                try
                {
                    _proxy.GetAwaiter().GetResult();

                }
                catch( OperationCanceledException ocex )
                {
                    monitor.Debug( $"Ignoring caught {ocex.GetType().Name}." );
                }
                _proxy?.Dispose();
                _proxy = null;
            }
            catch( Exception ex )
            {
                monitor.Error( $"Caught while stopping {nameof( BackgroundJobService )}.", ex );
            }
        }
    }
}
