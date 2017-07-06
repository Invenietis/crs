using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

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
}
