using CK.Core;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CrsConfigurationBuilder : ICrsConfiguration, IApplicationFeatureProvider<ControllerFeature>
    {
        IRequestRegistry _commands;
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

        internal IRequestRegistry Registry => _commands;
        internal CrsEndpointConfiguration Endpoints => _endpoints;

        public ICrsConfiguration AddCommands(Action<IRequestRegistry> registryConfiguration)
        {
            _commands = new DefaultRequestRegistry( new CKTraitContext("Crs") );

            registryConfiguration(Registry);
                
            _services.AddSingleton<IRequestRegistry>(Registry);

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
                    var crsEndpointControllerType = endpoint.Key.MakeGenericType(command.Type).GetTypeInfo();
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
