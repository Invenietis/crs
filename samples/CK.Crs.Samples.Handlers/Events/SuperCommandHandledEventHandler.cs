using CK.Crs.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class SuperCommandHandledEventHandler : CommandHandlerAsync<SuperCommandHandledEvent>
    {
        readonly IWebClientDispatcher _dispatcher;
        public SuperCommandHandledEventHandler(IWebClientDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected override Task<object> HandleCommandAsync(SuperCommandHandledEvent command, ICommandContext context)
        {
            _dispatcher.Send(command.Message, context);

            return Task.FromResult<object>(null);
        }
    }
}
