using CK.Core;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
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
        public static ICrsCoreBuilder AddCrs(this IServiceCollection services, Action<ICrsConfiguration> configuration)
        {
            CrsConfigurationBuilder feature = new CrsConfigurationBuilder(services);

            services.AddSingleton<ICommandDispatcher, DefaultCommandDispatcher>();
            services.AddMemoryCache();
            services.AddMvcCore(o =>
            {
                o.Conventions.Add(new CrsControllerNameConvention());
                o.Conventions.Add(new CrsActionConvention());
            })
            .AddJsonFormatters()
            .ConfigureApplicationPartManager(p =>
            {
                configuration(feature);
                p.FeatureProviders.Add(feature);
            });

            return new CrsCoreBuilder( services, feature );
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
            if (!ReflectionUtil.IsAssignableToGenericType(endpoint.GetGenericTypeDefinition(), typeof(ICrsEndpoint<>)) )
                throw new ArgumentException("The endpoint must implement ICrsEndpoint", nameof(endpoint));

            _currentConfiguredEndpoint = endpoint;
            
            return this;
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
        ICommandRegistry _commands;
        IServiceCollection _services;
        CrsEndpointConfiguration _endpoints;

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

        internal ICommandRegistry Registry => _commands;
        internal CrsEndpointConfiguration Endpoints => _endpoints;

        public ICrsConfiguration AddCommands(Action<ICommandRegistry> registryConfiguration)
        {
            _commands = new CommandRegistry( new CKTraitContext("Crs") );

            registryConfiguration(Registry);
                
            _services.AddSingleton<ICommandRegistry>(Registry);

            return this;
        }

        public ICrsConfiguration AddEndpoints(Action<ICrsEndpointConfigurationRoot> endpointConfiguration)
        {
            _endpoints = new CrsEndpointConfiguration( Registry );
            endpointConfiguration(_endpoints);
            return this;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (Registry == null) throw new InvalidOperationException("No CRS Commands configured. Consider calling AddCommands.");
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
