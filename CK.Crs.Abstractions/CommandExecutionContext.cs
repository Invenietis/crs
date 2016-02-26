using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CommandExecutionContext : ICommandExecutionContext
    {
        Func<ICommandExecutionContext, IExternalEventPublisher> _eventPublisherFactory;
  
        public CommandExecutionContext( Func<ICommandExecutionContext, IExternalEventPublisher> eventPublisher, IActivityMonitor monitor, object model, Guid commandId, bool longRunning, string callbackId, CancellationToken cancellationToken )
        {
            if( eventPublisher == null ) throw new ArgumentNullException( nameof( eventPublisher ) );

            _eventPublisherFactory = eventPublisher;

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

        public IActivityMonitor Monitor { get; set; }

        public CancellationToken CommandAborted { get; set; }

        IExternalEventPublisher _eventPublisherCacher;

        public IExternalEventPublisher ExternalEvents
        {
            get { return _eventPublisherCacher ?? (_eventPublisherCacher = _eventPublisherFactory( this )); }
        }
    }
}
