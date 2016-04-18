using System;
using System.Collections.Generic;
using System.Linq;
using CK.Crs.Pipeline;

namespace CK.Crs
{
    public class CommandReceiverConfiguration : IPipelineConfiguration
    {
        internal ICommandRegistry _registry;
        readonly HashSet<Type> _filters;

        public CommandReceiverConfiguration( ICommandRegistry registry, ICommandRouteCollection routes )
        {
            _registry = registry;
            _filters = new HashSet<Type>();

            Pipeline = new PipelineBuilder();

            Events = new PipelineEvents();
            Routes = routes; // new CommandRouteCollection( prefix );
        }

        public ICommandRouteCollection Routes { get; }

        public PipelineEvents Events { get; }

        /// <summary>
        /// Gets access to the pipeline
        /// </summary>
        public IPipelineBuilder Pipeline { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public CommandReceiverConfiguration AddFilter( Type filterType )
        {
            _filters.Add( filterType );
            return this;
        }

        /// <summary>
        /// Add a global filter to this <see cref="CommandReceiverConfiguration"/> that will be applied to any of command received.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CommandReceiverConfiguration AddFilter<T>() where T : ICommandFilter
        {
            return AddFilter( typeof( T ) );
        }

        /// <summary>
        /// Selects the commands from the <see cref="ICommandRegistry"/> this CommandReceiver is able to handle. 
        /// </summary>
        /// <param name="selection">A projection lambda to filter commands</param>
        /// <returns><see cref="CommandReceiverConfiguration"/></returns>
        public CommandReceiverConfiguration AddCommands(
            Func<ICommandRegistry, IEnumerable<CommandDescription>> selection,
            Action<ICommandRegistrationWithFilter> globalConfiguration = null )
        {
            foreach( var c in selection( _registry ) )
            {
                var registration = new CommandRegistration( Routes.AddCommandRoute( c ) );
                globalConfiguration?.Invoke( registration );
            }
            return this;
        }

        public ICommandConfiguration<ICommandRegistrationWithFilter> AddCommand( Type commandType )
        {
            var commandDescription = _registry.Registration.SingleOrDefault( c => c.CommandType == commandType );
            if( commandDescription == null )
            {
                throw new InvalidOperationException( $"Command {commandType.FullName} not found in global CommandRegistry. Make sure to register it in AddCommandReceiver options from ConfigureServices." );
            }
            return new CommandRegistration( Routes.AddCommandRoute( commandDescription ) );
        }


        /// <summary>
        /// Select a command from the <see cref="ICommandRegistry"/> this CommandReceiver will handle.
        /// </summary>
        /// <typeparam name="T">The type of the command</typeparam>
        /// <returns>Fluent <see cref="ICommandRegistration"/></returns>
        public ICommandConfiguration<ICommandRegistrationWithFilter> AddCommand<T>()
        {
            return AddCommand( typeof( T ) );
        }
    }
}