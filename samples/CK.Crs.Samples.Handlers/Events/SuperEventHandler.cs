using CK.Crs.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class SuperEventHandler : EventHandlerAsync<SuperEvent>
    {
        readonly IWebClientDispatcher _dispatcher;
        public SuperEventHandler(IWebClientDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected override Task HandleEventAsync(SuperEvent evt, ICommandContext context)
        {
            _dispatcher.Send(evt.Message, context);

            return Task.CompletedTask;
        }
    }
}
