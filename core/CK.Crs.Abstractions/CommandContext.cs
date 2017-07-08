using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public class CommandContext : ICommandContext
    {
        public CommandContext(Guid guid, IActivityMonitor activityMonitor, string callbackId, CancellationToken token = default( CancellationToken))
        {
            Id = guid;
            Monitor = activityMonitor;
            CallerId = callbackId;
            Aborted = token;
        }

        public Guid Id { get; }

        public IActivityMonitor Monitor { get; }

        public string CallerId { get; }

        public CancellationToken Aborted { get; }
    }
}