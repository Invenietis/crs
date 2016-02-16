using System;
using System.Threading;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public interface IMutableCommandContext
    {
        IActivityMonitor Monitor { get; }

        CancellationToken CommandAborted { get; }
    }

    public abstract class CommandContext : IMutableCommandContext
    {
        public CommandContext( IActivityMonitor monitor, object command, Guid commandId, bool longRunning, string callbackId, CancellationToken cancellationToken )
        {
            Monitor = monitor;
            CommandId = commandId;
            IsLongRunning = longRunning;
            CallbackId = callbackId;
            Command = command;
            CommandAborted = cancellationToken;
        }

        public Guid CommandId { get; }

        public object Command { get; }

        public string CallbackId { get; }

        public bool IsLongRunning { get; }

        public IActivityMonitor Monitor { get; private set; }

        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        public CancellationToken CommandAborted { get; private set; }

        public void Mutate( IMutableCommandContext ctx )
        {
            Monitor = ctx.Monitor;
            CommandAborted = ctx.CommandAborted;
        }
    }

    public class CommandContext<T> : CommandContext where T : class
    {
        public CommandContext( IActivityMonitor monitor, T command, Guid commandId, bool longRunning, string callbackId, CancellationToken cancellationToken ) :
            base( monitor, command, commandId, longRunning, callbackId, cancellationToken )
        {
            Command = command;
        }

        public new T Command { get; private set; }
    }
}
