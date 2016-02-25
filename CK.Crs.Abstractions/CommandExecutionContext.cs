using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CommandExecutionContext : IMutableCommand, ICommandExecutionContext
    {
        public CommandExecutionContext( IActivityMonitor monitor, object model, Guid commandId, bool longRunning, string callbackId, CancellationToken cancellationToken )
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

        void IMutableCommand.Mutate( IMutableCommand command )
        {
            Monitor = command.Monitor;
            CommandAborted = command.CommandAborted;
        }

        static Func<Event, Task> _delegate;

        public virtual Task PublishEventAsync<T>( T @event )
        {
            if( _delegate == null ) throw new InvalidOperationException( "An event emitter must be configured by calling CommandExecutionContext.SetEventEmitter" );

            var token = Monitor.DependentActivity().CreateToken();
            var eventEnvelope = new Event( token, @event, typeof(T));

            return _delegate( eventEnvelope );
        }


        public static void SetEventEmitter( Func<Event, Task> emitter )
        {
            _delegate = emitter;
        }
    }
}
