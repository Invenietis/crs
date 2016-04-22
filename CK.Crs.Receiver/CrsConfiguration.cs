using System;
using System.Collections.Generic;
using System.Linq;
using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Routing;

namespace CK.Crs.Runtime
{
    public class CrsConfiguration : ICrsConfiguration
    {
        internal CommandRouteCollection _routes;
        internal ICommandRegistry _registry;
        readonly HashSet<Type> _filters;
        string _path;
        public CrsConfiguration( string path, ICommandRegistry registry, CommandRouteCollection routes )
        {
            _path = path;
            _registry = registry;
            _routes = routes;

            _filters = new HashSet<Type>();

            Pipeline = new PipelineBuilder();
            Events = new PipelineEvents();
            TraitContext = new CKTraitContext( "CrsDefault" );
        }

        /// <summary>
        /// Gets or sets the <see cref="CKTraitContext"/>
        /// </summary>
        public CKTraitContext TraitContext { get; set; }

        public string ReceiverPath => _path;

        /// <summary>
        /// Gets the routes registration
        /// </summary>
        public IReadOnlyDictionary<CommandRoutePath, CommandRoute> Routes => _routes.RouteStorage;

        /// <summary>
        /// Gets the pipeline events configuration object.
        /// </summary>
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
        public CrsConfiguration AddFilter( Type filterType )
        {
            _filters.Add( filterType );
            return this;
        }

        /// <summary>
        /// Add a global filter to this <see cref="CrsConfiguration"/> that will be applied to any of command received.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CrsConfiguration AddFilter<T>() where T : ICommandFilter
        {
            return AddFilter( typeof( T ) );
        }

        /// <summary>
        /// Selects the commands from the <see cref="ICommandRegistry"/> this CommandReceiver is able to handle. 
        /// </summary>
        /// <param name="selection">A projection lambda to filter commands</param>
        /// <returns><see cref="CrsConfiguration"/></returns>
        public ICrsConfiguration AddCommands(
            Func<ICommandRegistry, IEnumerable<CommandDescription>> selection,
            Action<ICommandRegistrationWithFilter> globalConfiguration = null )
        {
            foreach( var c in selection( _registry ) )
            {
                var registration = new CommandRegistration( _routes.AddRoute( ReceiverPath, c ) );
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
            return new CommandRegistration( _routes.AddRoute( ReceiverPath, commandDescription ) );
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