using System;
using CK.Infrastructure.Commands;
using CK.Infrastructure.Commands;

namespace CK.Infrastructure.Commands
{
    public class DefaultReceiverOptions : ICommandReceiverOptions
    {
        public DefaultReceiverOptions( string routePrefix )
        {
            RoutePrefix = routePrefix;

        }
        public CommandRouteOptions CommandRouteOptions { get; } = new CommandRouteOptions();

        public string RoutePrefix { get; private set; }
    }
}