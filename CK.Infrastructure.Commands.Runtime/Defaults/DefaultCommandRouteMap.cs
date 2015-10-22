using System;
using System.Collections.Concurrent;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandRouteMap : ICommandRouteMap
    {
        ConcurrentDictionary<CommandRoutePath, CommandRouteRegistration> CommandRouteMap { get; } = new ConcurrentDictionary<CommandRoutePath, CommandRouteRegistration>();

        public ICommandRouteMap Register( CommandRouteRegistration registration )
        {
            CommandRouteMap.TryAdd( registration.Route, registration );
            return this;
        }

        public CommandRouteRegistration FindCommandRoute( string requestPath, ICommandReceiverOptions options )
        {
            CommandRouteRegistration registration = null;
            CommandRoutePath route = new CommandRoutePath( requestPath );
            CommandRouteMap.TryGetValue( route, out registration );
            return registration;
        }
    }
}