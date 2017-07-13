using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CK.Crs.Infrastructure;

namespace CK.Crs.Infrastructure
{
    class CrsEndpointConfiguration : ICrsEndpointConfigurationRoot, ICrsEndpointConfiguration
    {
        readonly IRequestRegistry _registry;
        readonly Dictionary<Type, ISet<RequestDescription>> _endpoints;

        Type _currentConfiguredEndpoint;
        private CrsModel _model;

        internal Dictionary<Type, ISet<RequestDescription>> ConfiguredEndpoints => _endpoints;

        internal CrsEndpointConfiguration( IRequestRegistry registry, CrsModel model )
        {
            _registry = registry ?? throw new ArgumentNullException( nameof( registry ), "You must first AddCommands before AddEndpoint." );
            _endpoints = new Dictionary<Type, ISet<RequestDescription>>();
            _model = model;
        }

        public ICrsEndpointConfiguration For( Type endpoint )
        {
            if( !ReflectionUtil.IsAssignableToGenericType( endpoint.GetGenericTypeDefinition(), typeof( ICrsReceiver<> ) ) )
                throw new ArgumentException( "The endpoint must implement ICrsEndpoint", nameof( endpoint ) );

            _currentConfiguredEndpoint = endpoint;

            return this;
        }


        public ICrsEndpointConfigurationRoot Apply( Func<RequestDescription, bool> filter )
        {
            Debug.Assert( _currentConfiguredEndpoint != null );

            ISet<RequestDescription> commands = new HashSet<RequestDescription>( _registry.Registration.Where( filter ) );
            _endpoints.Add( _currentConfiguredEndpoint, commands );

            _model.AddEndpoint( new CrsReceiverModel
            {
                ReceiverType = _currentConfiguredEndpoint,
                Requests = commands.ToArray()
            } );
            return this;
        }

    }
}
