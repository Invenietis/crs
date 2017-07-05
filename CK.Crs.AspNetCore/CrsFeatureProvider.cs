using CK.Core;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs
{
    public static class CrsCoreBuilderExtension
    {
        public static void AddCrs(this IMvcCoreBuilder builder, Action<ICrsConfiguration> configuration)
        {
            builder.Services.AddMemoryCache();

            builder.ConfigureApplicationPartManager(p =>
            {
                CrsConfigurationBuilder feature = new CrsConfigurationBuilder( builder.Services );
                configuration(feature);
                p.FeatureProviders.Add(feature);
            });
        }
    }

    public class CrsEndpointConfiguration : ICrsEndpointConfigurationRoot, ICrsEndpointConfiguration
    {
        readonly ICommandRegistry _registry;
        readonly Dictionary<Type, ISet<CommandDescription>> _endpoints;

        Type _currentConfiguredEndpoint;

        internal Dictionary<Type, ISet<CommandDescription>> ConfiguredEndpoints => _endpoints;

        public CrsEndpointConfiguration( ICommandRegistry registry )
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry), "You must first AddCommands before AddEndpoint.");
            _endpoints = new Dictionary<Type, ISet<CommandDescription>>();
        }

        public ICrsEndpointConfiguration For(Type endpoint)
        {
            if (!IsAssignableToGenericType(endpoint.GetGenericTypeDefinition(), typeof(ICrsEndpoint<>)) )
                throw new ArgumentException("The endpoint must implement ICrsEndpoint", nameof(endpoint));

            _currentConfiguredEndpoint = endpoint;
            return this;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.GetTypeInfo().IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.GetTypeInfo().BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public ICrsEndpointConfigurationRoot Apply(Func<CommandDescription, bool> filter)
        {
            Debug.Assert(_currentConfiguredEndpoint != null);
            
            ISet<CommandDescription> commands = new HashSet<CommandDescription>(_registry.Registration.Where(filter) );
            _endpoints.Add(_currentConfiguredEndpoint, commands);

            return this;
        }

    }

    public class CrsConfigurationBuilder : ICrsConfiguration, IApplicationFeatureProvider<ControllerFeature>
    {
        IServiceCollection _services;
        public CrsConfigurationBuilder( IServiceCollection services )
        {
            _services = services;
        }

        public ICrsConfiguration AddAmbientValues(Action<IAmbientValuesRegistration> ambientValuesConfiguration)
        {
            var config = new CrsAmbientValuesConfiguration( this, _services );
            var registration = config.Configure();
            ambientValuesConfiguration(registration);
            return this;
        }

        ICommandRegistry _commands;

        public ICrsConfiguration AddCommands(Action<ICommandRegistry> registryConfiguration)
        {
            _commands = new CommandRegistry( new CKTraitContext("Crs") );

            registryConfiguration(_commands);
                
            _services.AddSingleton<ICommandRegistry>(_commands);

            return this;
        }

        CrsEndpointConfiguration _endpoints;

        public ICrsConfiguration AddEndpoints(Action<ICrsEndpointConfigurationRoot> endpointConfiguration)
        {
            _endpoints = new CrsEndpointConfiguration( _commands );
            endpointConfiguration(_endpoints);
            return this;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (_commands == null) throw new InvalidOperationException("No CRS Commands configured. Consider calling AddCommands.");
            if (_endpoints == null) throw new InvalidOperationException("No CRS Endpoint configured. Consider calling AddEndpoints.");

            foreach (var endpoint in _endpoints.ConfiguredEndpoints)
            {
                foreach (var command in endpoint.Value)
                {
                    var crsEndpointControllerType = endpoint.Key.MakeGenericType(command.CommandType).GetTypeInfo();
                    feature.Controllers.Add(crsEndpointControllerType);
                }
            }
        }

        public void Build(ApplicationPartManager applicationParts)
        {
            applicationParts.FeatureProviders.Add(this);
        }

    }
}
