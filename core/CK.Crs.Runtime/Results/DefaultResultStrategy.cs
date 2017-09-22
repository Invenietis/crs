using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs
{
    public class DefaultResultStrategy : IResultStrategy
    {
        BroadcastResultReceiver _broadcastResultReceiver;
        DefaultResultReceiver _defaultResultReceiver;

        public DefaultResultStrategy( IClientDispatcher clientDispatcher )
        {
            ClientDispatcher = clientDispatcher;
        }

        public IClientDispatcher ClientDispatcher { get; }

        public virtual IResultReceiver GetResultReceiver( CommandModel model )
        {
            if( model.HasResultBroadcastTag() )
            {
                return _broadcastResultReceiver ?? (_broadcastResultReceiver = new BroadcastResultReceiver( ClientDispatcher ));
            }
            if( model.HasResultTag() )
            {
                return _defaultResultReceiver ?? (_defaultResultReceiver = new DefaultResultReceiver( ClientDispatcher ));
            }
            return null;
        }
    }
}
