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
        CrsEndpointConfiguration _endpoints;

        readonly IServiceCollection _services;
        readonly CrsModel _model;
        readonly CKTraitContext _traitContext;

        public CrsConfigurationBuilder( IServiceCollection services, string traitContextName = "Crs" )
        {
            _services = services;
            _traitContext = new CKTraitContext( traitContextName );
            _model = new CrsModel( _traitContext );
        }

        internal IServiceCollection Services => _services;
        internal ICommandRegistry Registry => _commands;
        internal CrsEndpointConfiguration Endpoints => _endpoints;

        ICrsConfiguration ICrsConfiguration.Commands( Action<ICommandRegistry> registryConfiguration )
        {
            _commands = new DefaultRequestRegistry( _traitContext );

            registryConfiguration( Registry );
            
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
