using CK.Core;
using CK.Crs.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CK.Crs
{
    public class CrsConfigurationBuilder : ICrsConfiguration
    {
        IRequestRegistry _commands;
        IServiceCollection _services;
        CrsEndpointConfiguration _endpoints;
        CrsModel _model;

        public CrsConfigurationBuilder( IServiceCollection services )
        {
            _services = services;
            _model = new CrsModel();
        }

        internal IRequestRegistry Registry => _commands;
        internal CrsEndpointConfiguration Endpoints => _endpoints;

        public ICrsConfiguration AddAmbientValues( Action<IAmbientValuesRegistration> ambientValuesConfiguration )
        {
            var config = new CrsAmbientValuesConfiguration( this, _services );
            var registration = config.Configure();
            ambientValuesConfiguration( registration );
            return this;
        }
      
        public ICrsConfiguration AddCommands( Action<IRequestRegistry> registryConfiguration )
        {
            _commands = new DefaultRequestRegistry( new CKTraitContext( "Crs" ) );

            registryConfiguration( Registry );

            _services.AddSingleton<IRequestRegistry>( Registry );
            
            return this;
        }

        public ICrsConfiguration AddEndpoints( Action<ICrsEndpointConfigurationRoot> endpointConfiguration )
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
