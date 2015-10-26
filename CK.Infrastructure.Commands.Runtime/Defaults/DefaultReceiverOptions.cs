using System;
namespace CK.Infrastructure.Commands
{
    public class DefaultReceiverOptions : ICommandReceiverOptions
    {
        public DefaultReceiverOptions( string routePrefix, ICommandRegistry map )
        {
            if( String.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );
            if( !routePrefix.StartsWith( "/" ) ) routePrefix = '/' + routePrefix;

            RoutePrefix = routePrefix;
            Registry = map;
        }

        public string RoutePrefix { get; private set; }

        public ICommandRegistry Registry { get; private set; }
    }
}