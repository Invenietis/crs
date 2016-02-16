using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public abstract class Command
    {
        public Command( IActivityMonitor monitor, object model, Guid commandId, bool longRunning, string callbackId, CancellationToken cancellationToken )
        {
            Monitor = monitor;
            CommandId = commandId;
            IsLongRunning = longRunning;
            CallbackId = callbackId;
            Model = model;
            CommandAborted = cancellationToken;
        }

        public Guid CommandId { get; }

        public object Model { get; }

        public string CallbackId { get; }

        public bool IsLongRunning { get; }

        public IActivityMonitor Monitor { get; protected set; }

        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        public CancellationToken CommandAborted { get; protected set; }
    }

    public class Command<T> : Command, IMutableCommand where T : class
    {
        public Command( IActivityMonitor monitor, T model, Guid commandId, bool longRunning, string callbackId, CancellationToken cancellationToken ) :
            base( monitor, model, commandId, longRunning, callbackId, cancellationToken )
        {
            Model = model;
        }

        public new T Model { get; private set; }

        void IMutableCommand.Mutate( IMutableCommand command )
        {
            Monitor = command.Monitor;
            CommandAborted = command.CommandAborted;
        }
    }
}
