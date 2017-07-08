using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CK.Crs
{
    public class CrsEndpointConfiguration : ICrsEndpointConfigurationRoot, ICrsEndpointConfiguration
    {
        readonly IRequestRegistry _registry;
        readonly Dictionary<Type, ISet<RequestDescription>> _endpoints;

        Type _currentConfiguredEndpoint;

        internal Dictionary<Type, ISet<RequestDescription>> ConfiguredEndpoints => _endpoints;

        public CrsEndpointConfiguration( IRequestRegistry registry )
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry), "You must first AddCommands before AddEndpoint.");
            _endpoints = new Dictionary<Type, ISet<RequestDescription>>();
        }

        public ICrsEndpointConfiguration For(Type endpoint)
        {
            if (!ReflectionUtil.IsAssignableToGenericType(endpoint.GetGenericTypeDefinition(), typeof(ICrsEndpoint<>)) )
                throw new ArgumentException("The endpoint must implement ICrsEndpoint", nameof(endpoint));

            _currentConfiguredEndpoint = endpoint;
            
            return this;
        }


        public ICrsEndpointConfigurationRoot Apply(Func<RequestDescription, bool> filter)
        {
            Debug.Assert(_currentConfiguredEndpoint != null);
            
            ISet<RequestDescription> commands = new HashSet<RequestDescription>(_registry.Registration.Where(filter) );
            _endpoints.Add(_currentConfiguredEndpoint, commands);

            return this;
        }

    }
}
