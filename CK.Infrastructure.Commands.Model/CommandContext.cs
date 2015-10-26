using System;
namespace CK.Infrastructure.Commands
{
    public abstract class CommandContext
    {
        public CommandContext( Guid commandId, bool longRunning, string callbackId )
        {
            CommandId = commandId;
            IsLongRunning = longRunning;
            CallbackId = callbackId;
        }

        public Guid CommandId { get; set; }

        //public IActivityMonitor Monitor { get; set; }

        public string CallbackId { get; }

        public bool IsLongRunning { get; set; }
    }

    public class CommandContext<T> : CommandContext where T : class
    {
        public CommandContext( T command, Guid commandId, bool longRunning, string callbackId ) : base( commandId, longRunning, callbackId )
        {
            Command = command;
        }

        public T Command { get; private set; }
    }
}
