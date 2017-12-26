using Rebus.Activation;
using Rebus.Handlers;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rebus.Bus;
using CK.Crs.Results;

namespace CK.Crs.Rebus
{
    sealed class GenericHandlerActivator : IHandlerActivator, IContainerAdapter
    {
        readonly ICommandRegistry _registry;
        readonly ICommandHandlerInvoker _invoker;
        readonly IResultReceiverProvider _resultStrategy;

        public GenericHandlerActivator( ICommandHandlerInvoker invoker, IResultReceiverProvider resultStrategy, ICommandRegistry registry )
        {
            _invoker = invoker ?? throw new ArgumentNullException( nameof( invoker ) );
            _resultStrategy = resultStrategy ?? throw new ArgumentNullException( nameof( resultStrategy ) );
            _registry = registry ?? throw new ArgumentNullException( nameof( registry ) );
        }

        public Task<IEnumerable<IHandleMessages<TMessage>>> GetHandlers<TMessage>(
            TMessage message,
            ITransactionContext transactionContext )
        {
            var res = new GenericRebusHandler<TMessage>( _invoker, _registry, _resultStrategy, _bus );
            return Task.FromResult<IEnumerable<IHandleMessages<TMessage>>>( new[] { res } );
        }

        IBus _bus;

        public void SetBus( IBus bus )
        {
            _bus = bus;
        }
    }
}
