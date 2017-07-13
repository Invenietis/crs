using System;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public class CrsCoreBuilder : ICrsCoreBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IRequestRegistry _registry;
        private readonly ICrsModel _model;

        public CrsCoreBuilder( CrsConfigurationBuilder builder  )
        {
            _services = builder.Services;
            _registry = builder.Registry;
            _model = builder.BuildModel();

            if( _model == null ) throw new ArgumentException( "CrsConfigurationBuilder must returns a valid ICrsModel." );
            _services.AddSingleton( _model );
        }

        public IServiceCollection Services => _services;

        public IRequestRegistry Registry => _registry;

        public ICrsModel Model => _model;
    }
}