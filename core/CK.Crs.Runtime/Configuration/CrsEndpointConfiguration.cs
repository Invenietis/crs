using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CK.Crs.Infrastructure;

namespace CK.Crs.Infrastructure
{
    class CrsEndpointConfiguration : ICrsEndpointConfigurationRoot, ICrsEndpointConfiguration
    {
        readonly ICommandRegistry _registry;
        readonly Dictionary<Type, ISet<CommandModel>> _endpoints;

        Type _currentConfiguredEndpoint;
        private CrsModel _model;

        internal Dictionary<Type, ISet<CommandModel>> ConfiguredEndpoints => _endpoints;

        internal CrsEndpointConfiguration( ICommandRegistry registry, CrsModel model )
        {
            _registry = registry ?? throw new ArgumentNullException( nameof( registry ), "You must first AddCommands before AddEndpoint." );
            _endpoints = new Dictionary<Type, ISet<CommandModel>>();
            _model = model;
        }

        public ICrsEndpointConfiguration For( Type endpoint )
        {
            //if( !ReflectionUtil.IsAssignableToGenericType( endpoint.GetGenericTypeDefinition(), typeof( IHttpCommandReceiver<> ) ) )
            //    throw new ArgumentException( "The endpoint must implement IHttpCommandReceiver", nameof( endpoint ) );

            _currentConfiguredEndpoint = endpoint;

            return this;
        }


        public ICrsEndpointConfigurationRoot Apply( Func<CommandModel, bool> filter )
        { 
            Debug.Assert( _currentConfiguredEndpoint != null );

            ISet<CommandModel> commands = new HashSet<CommandModel>( _registry.Registration.Where( filter ), new CommandModelComparer() );
            _endpoints.Add( _currentConfiguredEndpoint, commands );

            _model.AddEndpoint( new CrsReceiverModel( _model, _currentConfiguredEndpoint, commands ) );
            return this;
        }

    }
}
