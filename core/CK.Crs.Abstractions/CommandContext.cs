using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public class CommandContext : ICommandContext
    {
        public CommandContext( Guid guid, Type commandType, IActivityMonitor activityMonitor, string callbackId, CancellationToken token = default( CancellationToken ) )
        {
            CommandId = guid;
            Monitor = activityMonitor ?? throw new ArgumentNullException( nameof( activityMonitor ) );
            CallerId = callbackId ?? throw new ArgumentNullException( nameof( callbackId ) );
            Aborted = token;
        }

        public CommandContext( Guid guid, Type commandType,  IActivityMonitor activityMonitor, string callbackId, ICrsReceiverModel endpointModel, CancellationToken token = default( CancellationToken ) )
        {
            CommandId = guid;
            Monitor = activityMonitor ?? throw new ArgumentNullException( nameof( activityMonitor ) );
            CallerId = callbackId ?? throw new ArgumentNullException( nameof( callbackId ) );
            Aborted = token;

            ReceiverModel = endpointModel ?? throw new ArgumentNullException( nameof( endpointModel ) );
            Model = ReceiverModel.GetRequestDescription( commandType ?? throw new ArgumentNullException( nameof( commandType ) ) );
        }

        public Guid CommandId { get; }

        public IActivityMonitor Monitor { get; }

        public string CallerId { get; }

        public CancellationToken Aborted { get; }

        public ICrsReceiverModel ReceiverModel { get; }

        public RequestDescription Model { get; }
    }
}