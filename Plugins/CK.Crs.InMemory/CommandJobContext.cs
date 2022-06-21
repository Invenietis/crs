using System;
using System.Threading;
using CK.Core;

namespace CK.Crs.InMemory
{
    class CommandJobContext : ICommandContext
    {
        private readonly ICommandContext _commandContext;

        public ActivityMonitor.DependentToken MonitorToken { get; }

        public CommandJobContext( ICommandContext commandContext )
        {
            _commandContext = commandContext;
            MonitorToken = commandContext.Monitor.CreateDependentToken( $"Command {commandContext.CommandId}." );
            Monitor = new ActivityMonitor();
        }

        public string CommandId => _commandContext.CommandId;

        public IActivityMonitor Monitor { get; private set; }

        public CancellationToken Aborted => _commandContext.Aborted;

        public CallerId CallerId => _commandContext.CallerId;

        public ICommandModel Model => _commandContext.Model;

        public T GetFeature<T>() where T : class => _commandContext.GetFeature<T>();

        public void SetFeature<T>( T feature ) where T : class => _commandContext.SetFeature( feature );

        internal IDisposable ChangeMonitor()
        {
            return Monitor.StartDependentActivity( MonitorToken );
        }
    }
}
