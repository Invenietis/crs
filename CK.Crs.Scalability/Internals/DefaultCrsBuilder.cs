using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability
{
    class DefaultCrsBuilder : ICrsBuilder, ICommandReceiverConfigurationOutput, ICommandReceiverConfigurationInput
    {
        readonly IList<ICommandListener> _listeners;
        readonly IList<ICommandResponseDispatcher> _dispatchers;
        private IServiceProvider _services;

        public DefaultCrsBuilder(IServiceProvider services)
        {
            _services = services;
            _dispatchers = new List<ICommandResponseDispatcher>();
            _listeners = new List<ICommandListener>();
        }

        public IServiceProvider Services => _services;

        public IList<ICommandListener> Listeners => _listeners;
        public IList<ICommandResponseDispatcher> Dispatchers => _dispatchers;

        public CrsHandlerBuilder<CrsConfiguration> Handler => throw new NotImplementedException();

        ICommandReceiverConfigurationInput ICrsBuilder.Input => this;

        ICommandReceiverConfigurationOutput ICrsBuilder.Output => this;

        void ICommandReceiverConfigurationOutput.AddDispatcher(ICommandResponseDispatcher dispatcher)
        {
            Dispatchers.Add(dispatcher);
        }

        void ICommandReceiverConfigurationInput.AddListener(ICommandListener listener)
        {
            Listeners.Add(listener);
        }
    }
}
