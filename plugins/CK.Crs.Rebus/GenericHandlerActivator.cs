using Rebus.Activation;
using Rebus.Handlers;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rebus.Bus;

namespace CK.Crs.Rebus
{
    sealed class GenericHandlerActivator : IHandlerActivator, IContainerAdapter
    {
        readonly ICommandRegistry _registry;
        readonly Lazy<ICommandHandlerInvoker> _invoker;
        readonly Lazy<IResultStrategy> _resultStrategy;

        public GenericHandlerActivator( Lazy<ICommandHandlerInvoker> invoker, Lazy<IResultStrategy> resultStrategy, ICommandRegistry registry )
        {
            _invoker = invoker ?? throw new ArgumentNullException( nameof( invoker ) );
            _resultStrategy = resultStrategy ?? throw new ArgumentNullException( nameof( resultStrategy ) );
            _registry = registry ?? throw new ArgumentNullException( nameof( registry ) );
        }

        public Task<IEnumerable<IHandleMessages<TMessage>>> GetHandlers<TMessage>(
            TMessage message,
            ITransactionContext transactionContext )
        {
            var res = new GenericRebusHandler<TMessage>( _invoker.Value, _registry, _resultStrategy.Value, _bus );
            return Task.FromResult<IEnumerable<IHandleMessages<TMessage>>>( new[] { res } );
        }

        global::Rebus.Bus.IBus _bus;

        public void SetBus( global::Rebus.Bus.IBus bus )
        {
            _bus = bus;
        }
    }
}
