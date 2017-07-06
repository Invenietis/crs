using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public class CommandExecutionContext : ICommandExecutionContext
    {
        public CommandExecutionContext(Guid guid, ActivityMonitor activityMonitor, string callbackId)
        {
            Id = guid;
            Monitor = activityMonitor;
            CallbackId = callbackId;
        }

        public Guid Id { get; }

        public IActivityMonitor Monitor { get; }

        public string CallbackId { get; }

        public CancellationToken CommandAborted { get; }
    }
}