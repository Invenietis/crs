using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public class CommandExecutionContext : ICommandContext
    {
        public CommandExecutionContext(Guid guid, IActivityMonitor activityMonitor, string callbackId, CancellationToken token = default( CancellationToken))
        {
            Id = guid;
            Monitor = activityMonitor;
            CallbackId = callbackId;
            CommandAborted = token;
        }

        public Guid Id { get; }

        public IActivityMonitor Monitor { get; }

        public string CallbackId { get; }

        public CancellationToken CommandAborted { get; }
    }
}