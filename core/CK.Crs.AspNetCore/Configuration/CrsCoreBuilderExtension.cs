using CK.Core;
using CK.Crs.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CK.Crs
{
    public static class CrsCoreBuilderExtension
    {
        public static ICrsCoreBuilder AddCrs( this IServiceCollection services, Action<ICrsConfiguration> configuration )
        {
            CrsConfigurationBuilder builder = new CrsConfigurationBuilder( services );

            services.AddSingleton<IBus, DefaultBus>();
            services.AddSingleton<ICommandDispatcher>( s => s.GetRequiredService<IBus>() );
            services.AddSingleton<IEventPublisher>( s => s.GetRequiredService<IBus>() );

            services.AddMvcCore( o =>
             {
                 o.Conventions.Add( new CrsControllerNameConvention() );
                 o.Conventions.Add( new CrsActionConvention() );
             } )
            .AddJsonFormatters()
            .ConfigureApplicationPartManager( p =>
             {
                 configuration( builder );
                 ICrsModel model = builder.BuildModel();
                 if( model == null ) throw new ArgumentException( "CrsConfigurationBuilder must returns a valid ICrsModel." );

                 services.AddSingleton( model );

                 CrsFeature feature = new CrsFeature( model );
                 p.FeatureProviders.Add( feature );
             } );

            return new CrsCoreBuilder( services, builder );
        }
    }
}
