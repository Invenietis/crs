using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class DefaultCrsEndpoint<T> : ICrsEndpoint<T> where T : class
    {
        readonly ICommandDispatcher _dispatcher;

        public DefaultCrsEndpoint( ICommandDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public virtual async Task<Response> ReceiveCommand( T command, IActivityMonitor monitor, string callerId )
        {
            var context = new CommandContext( Guid.NewGuid(), monitor, callerId );
            await _dispatcher.SendAsync( command, context );

            return new DeferredResponse( context.Id, context.CallerId );
        }
    }
}
