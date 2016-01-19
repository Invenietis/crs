using System;
using System.Collections.Generic;

namespace CK.Infrastructure.Commands
{
    internal class CommandRegistry : ICommandRegistry
    {
        Dictionary<CommandRoutePath, CommandDescriptor> Map { get; } = new Dictionary<CommandRoutePath, CommandDescriptor>();

        public ICommandRegistry Register( CommandDescriptor registration )
        {
            Map.Add( registration.Route, registration );
            return this;
        }

        public CommandDescriptor Find( string commandReceiverPath, string commandName )
        {
            CommandDescriptor registration = null;
            CommandRoutePath commandRoute = new CommandRoutePath( commandReceiverPath, commandName );

            if( commandRoute.IsValidFor( commandReceiverPath ) ) Map.TryGetValue( commandRoute, out registration );
            return registration;
        }
    }
}