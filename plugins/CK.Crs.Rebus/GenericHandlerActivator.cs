using Rebus.Activation;
using Rebus.Handlers;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Rebus
{
    sealed class GenericHandlerActivator : IHandlerActivator
    {
        readonly IRequestRegistry _registry;
        readonly Lazy<IRequestHandlerFactory> _factory;
        public GenericHandlerActivator( Lazy<IRequestHandlerFactory> factory, IRequestRegistry registry )
        {
            _factory = factory ?? throw new ArgumentNullException( nameof( factory ) );
            _registry = registry ?? throw new ArgumentNullException( nameof( registry ) );
        }

        public Task<IEnumerable<IHandleMessages<TMessage>>> GetHandlers<TMessage>(
            TMessage message,
            ITransactionContext transactionContext )
        {
            var res = new GenericRebusHandler<TMessage>( _factory.Value, _registry );
            return Task.FromResult<IEnumerable<IHandleMessages<TMessage>>>( new[] { res } );
        }
    }
}
