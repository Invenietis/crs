using System;
using System.Collections.Generic;
using System.Linq;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public class CommandReceiverConfiguration : CK.Core.IFluentInterface
    {
        internal ICommandRegistry _registry;
        internal ICommandRouteCollection _routes;
        public CommandReceiverConfiguration( ICommandRegistry registry, ICommandRouteCollection routes )
        {
            _registry = registry;
            _routes = routes;
        }

        public CommandReceiverConfiguration AddFilter<T>() where T : ICommandFilter
        {

            return this;
        }

        /// <summary>
        /// Selects the commands from the <see cref="ICommandRegistry"/> this CommandReceiver is able to handle. 
        /// </summary>
        /// <param name="selection">A projection lambda to filter commands</param>
        /// <returns><see cref="CommandReceiverConfiguration"/></returns>
        public CommandReceiverConfiguration AddCommands( Func<ICommandRegistry, IEnumerable<CommandDescriptor>> selection )
        {
            foreach( var c in selection( _registry ) ) _routes.AddCommandRoute( c );
            return this;
        }

        /// <summary>
        /// Select a command from the <see cref="ICommandRegistry"/> this CommandReceiver will handle.
        /// </summary>
        /// <typeparam name="T">The type of the command</typeparam>
        /// <returns>Fluent <see cref="ICommandRegistration"/></returns>
        public ICommandRegistration AddCommand<T>()
        {
            var commandDescription = _registry.Registration.SingleOrDefault( c => c.CommandType == typeof( T ) );
            if( commandDescription == null )
                throw new InvalidOperationException( $"Command {typeof( T ).FullName} not found in global CommandRegistry. Make sure to register it in AddCommandReceiver options from ConfigureServices." );
            _routes.AddCommandRoute( commandDescription );
            return new CommandRegistration<T>( commandDescription );
        }
    }
}