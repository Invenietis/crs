using System;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public class CrsCoreBuilder : ICrsCoreBuilder
    {
        private IServiceCollection _services;
        private CrsConfigurationBuilder _feature;

        public CrsCoreBuilder( IServiceCollection services, CrsConfigurationBuilder builder )
        {
            _services = services;
            _feature = builder;
        }

        public IServiceCollection Services => _services;

        public IRequestRegistry Registry => _feature.Registry;
    }
}