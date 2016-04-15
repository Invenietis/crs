using System;
using System.Collections.Generic;
using System.Linq;
using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Pipeline;

namespace CK.Crs
{
    public interface IPipelineConfiguration
    {
        /// <summary>
        /// Gets the <see cref="ICommandRouteCollection"/> 
        /// </summary>
        ICommandRouteCollection Routes { get; }

        /// <summary>
        /// Gets the <see cref="PipelineEvents"/> 
        /// </summary>
        PipelineEvents Events { get; }

        FactoryConfiguration Factories { get; }
    }

    public class FactoryConfiguration
    {
        public Func<ICommandExecutionContext, IExternalEventPublisher> ExternalEvents { get; set; }
        public Func<ICommandExecutionContext, ICommandScheduler> CommandScheduler { get; set; }
    }

    public class CommandReceiverConfiguration : IPipelineConfiguration
    {
        internal ICommandRegistry _registry;
        readonly HashSet<Type> _filters;

        public CommandReceiverConfiguration( string prefix, ICommandRegistry registry )
        {
            _registry = registry;
            _filters = new HashSet<Type>();

            Pipeline = new PipelineBuilder();
            Pipeline.Clear().UseDefault();

            Events = new PipelineEvents();
            Routes = new CommandRouteCollection( prefix );

            Factories = new FactoryConfiguration();
        }

        public FactoryConfiguration Factories { get; }

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