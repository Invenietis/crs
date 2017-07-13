using CK.Crs.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class SuperEventHandler : IEventHandler<SuperEvent>
    {
        readonly IWebClientDispatcher _dispatcher;
        public SuperEventHandler( IWebClientDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public Task HandleAsync( SuperEvent evt, ICommandContext context )
        {
            return _dispatcher.Send( "SuperWebClientEvent", evt.Message, context );
        }
    }
}
