using CK.Core;
using System;

namespace CK.Crs.Results
{
    class DefaultResultReceiverProvider : IResultReceiverProvider
    {
        BroadcastResultReceiver _broadcastResultReceiver;
        SendResultReceiver _defaultResultReceiver;

        public DefaultResultReceiverProvider( IResultDispatcherSelector resultDispatcherSelector )
            => ResultDispatcherSelector = resultDispatcherSelector ?? throw new ArgumentNullException( nameof( resultDispatcherSelector ) );

        public IResultDispatcherSelector ResultDispatcherSelector { get; }

        public virtual IResultReceiver GetResultReceiver( ICommandContext context )
        {
            using( context.Monitor.OpenTrace( "Obtaining the result receiver for dispatching result to the client." ) )
            {
                if( context.Model.HasResultBroadcastTag() )
                {
                    context.Monitor.Trace( $"Selecting a broadcast result strategy thanks to the following CommandModel tags: {context.Model.Tags.ToString()}" );
                    return _broadcastResultReceiver ?? (_broadcastResultReceiver = new BroadcastResultReceiver( ResultDispatcherSelector ));
                }

                context.Monitor.Trace( $"Selecting a single result strategy thanks to the following CommandModel tags: {context.Model.Tags.ToString()}" );
                return _defaultResultReceiver ?? (_defaultResultReceiver = new SendResultReceiver( ResultDispatcherSelector ));
            }
        }
    }
}
