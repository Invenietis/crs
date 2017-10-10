using CK.Core;
using CK.Crs;
using CK.Crs.Infrastructure;
using System;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class CrsCoreBuilderExtension
    {
        public static CrsMvcCoreBuilder AddCrs( this IServiceCollection services, Action<ICrsConfiguration> configuration )
        {
            var builder = services.AddCrsCore( configuration );
            var mvcBuilder = builder.AddMvcCoreEndpoint();
            return new CrsMvcCoreBuilder
            {
                CrsBuilder = builder,
                MvcBuilder = mvcBuilder
            };
        }

        public static IMvcCoreBuilder AddMvcCoreEndpoint( this ICrsCoreBuilder builder )
        {
            var model = builder.Model;
            return builder.Services
                .AddMvcCore( o =>
                {
                    o.Conventions.Add( new CrsControllerNameConvention( model ) );
                    o.Conventions.Add( new CrsActionConvention( model ) );
                } )
                .AddJsonFormatters()
                .ConfigureApplicationPartManager( p =>
                {
                    CrsFeature feature = new CrsFeature( model );
                    p.FeatureProviders.Add( feature );
                } );
        }
    }
}
