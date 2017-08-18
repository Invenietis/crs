using CK.Core;
using CK.Crs.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CK.Crs
{
    public class CrsConfigurationBuilder : ICrsConfiguration
    {
        ICommandRegistry _commands;
        IServiceCollection _services;
        CrsEndpointConfiguration _endpoints;
        CrsModel _model;
        
        public CrsConfigurationBuilder( IServiceCollection services )
        {
            _services = services;
            _model = new CrsModel();
        }

        internal IServiceCollection Services => _services;
        internal ICommandRegistry Registry => _commands;
        internal CrsEndpointConfiguration Endpoints => _endpoints;

        ICrsConfiguration ICrsConfiguration.Commands( Action<ICommandRegistry> registryConfiguration )
        {
            _commands = new DefaultRequestRegistry( new CKTraitContext( "Crs" ) );

            registryConfiguration( Registry );

            _services.AddSingleton<ICommandRegistry>( Registry );
            
            return this;
        }

        ICrsConfiguration ICrsConfiguration.Endpoints( Action<ICrsEndpointConfigurationRoot> endpointConfiguration )
        {
            _endpoints = new CrsEndpointConfiguration( Registry, _model );
            endpointConfiguration( _endpoints );

            return this;
        }

        public virtual ICrsModel BuildModel()
        {
            return _model;
        }

    }
}
