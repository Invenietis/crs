using CK.Core;
using CK.Crs.Responses;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{

    class DispatcherCommandContext<TCommand, TResult> : CommandContext
    {
        private readonly object _command;
        private readonly IServiceProvider _serviceProvider;

        public DispatcherCommandContext( Guid commandId, TCommand command, ICommandModel commandModel, CallerId callerId, IServiceProvider serviceProvider ) :
            base
            (
                commandId.ToString(),
                new ActivityMonitor(),
                commandModel,
                callerId
            )
        {
            _command = command;
            _serviceProvider = serviceProvider;

            if( commandModel.HasFireAndForgetTag() )
            {
                TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
                SetFeature( tcs );
            }

        }

        internal async Task<TResult> Receive()
        {
            using( var serviceScope = _serviceProvider.CreateScope() )
            {
                try
                {
                    var receiver = ServiceContainerExtension.GetService<ICommandReceiver>( serviceScope.ServiceProvider, true );
                    var response = await receiver.ReceiveCommand( _command, this ).ConfigureAwait( false );
                    return await HandleResponse( response ).ConfigureAwait( false );
                }
                catch( Exception ex )
                {
                    Monitor.Error( ex );
                    throw;
                }
            }

        }

        private async Task<TResult> HandleResponse( Response response )
        {
            if( response is ErrorResponse errorResponse )
            {
                Monitor.Trace( "Error response received." );
                throw new Exception( errorResponse.Payload.ToString() );
            }

            if( response is InvalidResponse invalidResponse )
            {
                Monitor.Trace( "Error response received." );
                throw new Exception( invalidResponse.Payload.ToString() );
            }

            if( response is Response<TResult> resultResponse )
            {
                Monitor.Trace( "Response received." );
                return resultResponse.Payload;
            }

            if( response is DeferredResponse deferredResponse )
            {
                Monitor.Trace( "Deferred response received. Registers to the callback" );

                var feature = GetFeature<TaskCompletionSource<TResult>>();
                if( feature == null )
                {
                    Monitor.Warn( "Unable to find the related TCS. Returns an empty result." );
                    return default;
                }

                var registration = Aborted.Register( feature.SetCanceled );
                try
                {
                    return await feature.Task.ConfigureAwait( false );
                }
                catch( Exception ex )
                {
                    Monitor.Error( ex );
                    throw;
                }
                finally
                {
                    registration.Dispose();
                }
            }

            throw new NotSupportedException( "Other types of Responses are not supported." );
        }
    }
}
