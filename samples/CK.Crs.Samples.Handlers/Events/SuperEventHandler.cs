using CK.Crs.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class SuperEventHandler : IEventHandler<SuperEvent>
    {
        readonly IClientDispatcher _dispatcher;
        public SuperEventHandler( IClientDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public Task HandleAsync( SuperEvent evt, ICommandContext context )
        {
            return _dispatcher.Send( "SuperWebClientEvent", evt.Message, context );
        }
    }
}
