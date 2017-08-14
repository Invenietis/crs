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
            _services.AddSingleton<IRequestHandlerFactory, DefaultRequestHandlerFactory>();
            _services.AddSingleton<ICommandSender, DefaultCommandSender>();
            _services.AddSingleton<IEventPublisher, DefaultEventPublisher>();
            _services.AddSingleton<IBus, DefaultBus>();
            
            // TODO: this should be added only if configured.
            _services.AddSingleton<IClientEventStore, InMemoryLiveEventStore>();

            _model = new CrsModel();
        }

        internal IServiceCollection Services => _services;
        internal IRequestRegistry Registry => _commands;
        internal CrsEndpointConfiguration Endpoints => _endpoints;

        public ICrsConfiguration AddCommands( Action<IRequestRegistry> registryConfiguration )
        {
            _commands = new DefaultRequestRegistry( new CKTraitContext( "Crs" ) );

            registryConfiguration( Registry );

            _services.AddSingleton<IRequestRegistry>( Registry );
            
            return this;
        }

        public ICrsConfiguration AddReceivers( Action<ICrsEndpointConfigurationRoot> endpointConfiguration )
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
