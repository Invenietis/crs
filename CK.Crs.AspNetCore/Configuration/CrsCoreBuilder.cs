using System;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    internal class CrsCoreBuilder : ICrsCoreBuilder
    {
        private IServiceCollection _services;
        private CrsConfigurationBuilder _feature;

        public CrsCoreBuilder(IServiceCollection services, CrsConfigurationBuilder feature)
        {
            _services = services;
            _feature = feature;
        }

        public IServiceCollection Services => _services;

        public ICommandRegistry Registry => _feature.Registry;
    }
}