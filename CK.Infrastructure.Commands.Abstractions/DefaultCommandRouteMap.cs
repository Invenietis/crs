using System;
using System.Collections.Concurrent;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandRouteMap : ICommandRouteMap
    {
        readonly ICommandTypeSelector _commandTypeSelector;

        ConcurrentDictionary<CommandRoutePath, CommandRouteRegistration> CommandRouteMap { get; } = new ConcurrentDictionary<CommandRoutePath, CommandRouteRegistration>();

        public DefaultCommandRouteMap( ICommandTypeSelector commandTypeSelector )
        {
            _commandTypeSelector = commandTypeSelector;
        }

        public CommandRouteRegistration FindCommandRoute( string requestPath, ICommandReceiverOptions options )
        {
            CommandRouteRegistration registration = null;

            // The registration key includes the whole request path.
            // The key is normalized as a lowercase string.

            CommandRoutePath route = new CommandRoutePath( requestPath );
            CommandRouteMap.TryGetValue( route, out registration );
            if( registration == null && route.IsValid( options.RoutePrefix ) )
            {
                registration = new CommandRouteRegistration
                {
                    Route = route,
                    CommandType = _commandTypeSelector.DetermineCommandType( options, route )
                };
                Register( registration );
            }
            return registration;
        }

        public ICommandRouteMap Register( CommandRouteRegistration registration )
        {
            CommandRouteMap.TryAdd( registration.Route, registration );
            return this;
        }
    }
}