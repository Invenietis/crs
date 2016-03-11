﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CommandExecutionContext : ICommandExecutionContext
    {
        Lazy<IExternalEventPublisher> _eventPublisherLazy;
        Lazy<ICommandScheduler> _cSchedulerLazy;

        public CommandExecutionContext( 
            Func<ICommandExecutionContext, IExternalEventPublisher> eventPublisher, 
            Func<ICommandExecutionContext, ICommandScheduler> commandScheduler, 
            IActivityMonitor monitor, 
            object model, 
            Guid commandId, 
            bool longRunning, 
            string callbackId,
            ClaimsPrincipal user,
            CancellationToken cancellationToken )
        {
            if( eventPublisher == null ) throw new ArgumentNullException( nameof( eventPublisher ) );
            if( commandScheduler == null ) throw new ArgumentNullException( nameof( commandScheduler ) );

            _eventPublisherLazy = new Lazy<IExternalEventPublisher>( () => eventPublisher( this ) );
            _cSchedulerLazy = new Lazy<ICommandScheduler>( () => commandScheduler( this ) );

            Monitor = monitor;
            CommandId = commandId;
            IsLongRunning = longRunning;
            CallbackId = callbackId;
            Model = model;
            User = user;
            CommandAborted = cancellationToken;
        }

        public Guid CommandId { get; }

        public object Model { get; }

        public string CallbackId { get; }

        public bool IsLongRunning { get; }

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

        public ClaimsPrincipal User { get; }
    }
}
