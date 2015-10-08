using System;
using System.Collections.Generic;

namespace CK.Infrastructure.Commands
{
    internal class DefaultCommandRouteMap : ICommandRouteMap
    {
        readonly ICommandTypeSelector _commandTypeSelector;
        public DefaultCommandRouteMap( ICommandTypeSelector commandTypeSelector )
        {
            _commandTypeSelector = commandTypeSelector;
        }

        IDictionary<string, CommandRouteRegistration> CommandRouteMap { get; } = new Dictionary<string, CommandRouteRegistration>();

        public CommandRouteRegistration FindCommandRoute( string requestPath, ICommandReceiverOptions options )
        {
            CommandRouteRegistration registration;
            CommandRouteMap.TryGetValue( requestPath.ToLowerInvariant(), out registration );
            if( registration == null )
            {
                if( IsCommandRoute( requestPath, options ) )
                {
                    registration = new CommandRouteRegistration
                    {
                        Route = requestPath,
                        CommandType = _commandTypeSelector.DetermineCommandType( options, requestPath )
                    };
                    if( options.CommandRouteOptions.AddConventionRoutesToMap )
                    {
                        CommandRouteMap.Add( registration.Route, registration );
                    }
                }
            }
            return registration;
        }

        protected virtual bool IsCommandRoute( string requestPath, ICommandReceiverOptions options )
        {
            return requestPath.StartsWith( options.RoutePrefix );
        }

        public ICommandRouteMap Register( CommandRouteRegistration route )
        {
            CommandRouteMap.Add( route.Route, route );
            return this;
        }
    }
}