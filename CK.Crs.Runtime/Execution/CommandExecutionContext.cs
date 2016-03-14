using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class CommandExecutionContext : ICommandExecutionContext
    {
        readonly Lazy<IExternalEventPublisher> _eventPublisherLazy;
        readonly Lazy<ICommandScheduler> _cSchedulerLazy;

        public CommandExecutionContext( CommandAction action, IActivityMonitor monitor, CancellationToken cancellationToken,
            Func<ICommandExecutionContext, IExternalEventPublisher> eventPublisher,
            Func<ICommandExecutionContext, ICommandScheduler> commandScheduler )
        {
            Action = action;
            Monitor = monitor;
            CommandAborted = cancellationToken;

            _eventPublisherLazy = new Lazy<IExternalEventPublisher>( () => eventPublisher( this ) );
            _cSchedulerLazy = new Lazy<ICommandScheduler>( () => commandScheduler( this ) );
        }

        public CommandAction Action { get; }

        public IActivityMonitor Monitor { get; set; }

        public CancellationToken CommandAborted { get; set; }

        public IExternalEventPublisher ExternalEvents
        {
            get { return _eventPublisherLazy.Value; }
        }

        public ICommandScheduler Scheduler
        {
            get { return _cSchedulerLazy.Value; }
        }
    }
}
