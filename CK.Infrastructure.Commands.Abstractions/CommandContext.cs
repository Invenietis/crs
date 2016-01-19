using System;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public abstract class CommandContext
    {
        public CommandContext( IActivityMonitor monitor, object command, Guid commandId, bool longRunning, string callbackId )
        {
            Monitor = monitor;
            CommandId = commandId;
            IsLongRunning = longRunning;
            CallbackId = callbackId;
            Command = command;
        }

        public Guid CommandId { get; }

        public object Command { get; }

        public IActivityMonitor Monitor { get; set; }

        public string CallbackId { get; }

        public bool IsLongRunning { get; }
    }

    public class CommandContext<T> : CommandContext where T : class
    {
        public CommandContext( IActivityMonitor monitor, T command, Guid commandId, bool longRunning, string callbackId ) :
            base( monitor, command, commandId, longRunning, callbackId )
        {
            Command = command;
        }

        public new T Command { get; private set; }
    }
}
