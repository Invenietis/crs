using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public class CommandContext : ICommandContext
    {
        public CommandContext( Guid guid, Type commandType, ICrsReceiverModel endpointModel, IActivityMonitor activityMonitor, string callbackId, CancellationToken token = default( CancellationToken ) )
        {
            CommandId = guid;
            ReceiverModel = endpointModel;
            Monitor = activityMonitor;
            CallerId = callbackId;
            Aborted = token;
            Model = ReceiverModel.GetRequestDescription( commandType );
        }

        public Guid CommandId { get; }

        public IActivityMonitor Monitor { get; }

        public string CallerId { get; }

        public CancellationToken Aborted { get; }

        public ICrsReceiverModel ReceiverModel { get; }

        public RequestDescription Model { get; }
    }
}