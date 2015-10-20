using System;
namespace CK.Infrastructure.Commands
{
    public class DefaultReceiverOptions : ICommandReceiverOptions
    {
        public DefaultReceiverOptions( string routePrefix )
        {
            if( String.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );
            if( !routePrefix.StartsWith( "/" ) ) routePrefix = '/' + routePrefix;

            RoutePrefix = routePrefix;
        }

        public CommandRouteOptions CommandRouteOptions { get; } = new CommandRouteOptions();

        public string RoutePrefix { get; private set; }
    }
}