using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class DefaultResultStrategy : IResultStrategy
    {
        public DefaultResultStrategy( IClientDispatcher clientDispatcher)
        {
            ClientDispatcher = clientDispatcher;
        }

        public IClientDispatcher ClientDispatcher { get; }

        public virtual IResultReceiver GetResultReceiver( CommandModel model )
        {
            if( model.HasResultBroadcastTag() )
            {
                return new BroadcastResultReceiver( ClientDispatcher );
            }
            if( model.HasResultTag() )
            {
                return new DefaultResultReceiver( ClientDispatcher );
            }
            return null;
        }
    }

    class BroadcastResultReceiver : IResultReceiver
    {
        private readonly IClientDispatcher _dispatcher;

        public BroadcastResultReceiver( IClientDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public Task ReceiveResult( object result, ICommandContext context )
        {
            var response = new Response( ResponseType.Synchronous, context.CommandId )
            {
                Payload = result
            };
            _dispatcher.Broadcast( response );
            return Task.FromResult( response );
        }
    }
}
