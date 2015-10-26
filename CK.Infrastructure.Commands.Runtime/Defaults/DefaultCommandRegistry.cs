using System;
using System.Collections.Generic;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandRegistry : ICommandRegistry
    {
        Dictionary<CommandRoutePath, CommandDescriptor> Map { get; } = new Dictionary<CommandRoutePath, CommandDescriptor>();

        public ICommandRegistry Register( CommandDescriptor registration )
        {
            Map.Add( registration.Route, registration );
            return this;
        }

        public CommandDescriptor Find( string requestPath )
        {
            CommandDescriptor registration = null;
            CommandRoutePath route = new CommandRoutePath( requestPath );
            Map.TryGetValue( route, out registration );
            return registration;
        }
    }
}