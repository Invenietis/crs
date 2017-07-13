using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public class CommandContext : ICommandContext
    {
        public CommandContext( Guid guid, ICrsReceiverModel endpointModel, IActivityMonitor activityMonitor, string callbackId, CancellationToken token = default( CancellationToken ) )
        {
            CommandId = guid;
            Receiver = endpointModel;
            Monitor = activityMonitor;
            CallerId = callbackId;
            Aborted = token;
        }

        public Guid CommandId { get; }

        public IActivityMonitor Monitor { get; }

        public string CallerId { get; }

        public CancellationToken Aborted { get; }

        public ICrsReceiverModel Receiver { get; }
    }
}