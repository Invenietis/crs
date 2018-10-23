using CK.Crs;
using CK.Crs.Owin;
using CK.Crs.Configuration;
using Newtonsoft.Json;
using System;
using Owin;
using Microsoft.Owin;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsCoreBuilderExtension
    {
        public static ICrsCoreBuilder AddCrs( this IServiceCollection services, Action<ICommandRegistry> commandsConfigurationn )
        {
            var builder = services.AddCrsCore( commandsConfigurationn );
            return builder.AddOwinHosting();
        }

        public static ICrsCoreBuilder AddOwinHosting( this ICrsCoreBuilder builder )
        {
            builder.Services.AddSingleton<ICommandFilterProvider, FilterProvider>();
            builder.Services.AddSingleton<ICommandFilter, AmbientValuesValidationFilter>();
            builder.Services.AddSingleton<ICommandFilter, ModelValidationFilter>();
            return builder;
        }

        /// <summary>
        /// Maps a default CRS endpoint to the given <paramref name="crsPath"/> that accepts all comamnds, validate ambient values, and valide command model.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="crsPath"></param>
        /// <returns></returns>
        public static IAppBuilder UseCrsOwin( this IAppBuilder app, PathString crsPath, IServiceProvider applicationServices )
        {
            return app.UseCrsOwin( crsPath, applicationServices, null );
        }

        public static IAppBuilder UseCrsOwin( this IAppBuilder app, string crsPath, IServiceProvider applicationServices )
        {
            return app.UseCrsOwin( new PathString( crsPath ), applicationServices, null );
        }

        public static IAppBuilder UseCrsOwin( this IAppBuilder app, string crsPath, IServiceProvider applicationServices, Action<ICrsEndpointConfiguration> configuration )
        {
            return app.UseCrsOwin( new PathString( crsPath ), applicationServices, configuration );
        }

        public static IAppBuilder UseCrsOwin( this IAppBuilder app, PathString crsPath, IServiceProvider applicationServices, Action<ICrsEndpointConfiguration> configuration )
        {
            return app.Map( crsPath, crsApp =>
            {

                ICrsModel model = applicationServices.GetRequiredService<ICrsModel>();
                ICommandRegistry registry = applicationServices.GetRequiredService<ICommandRegistry>();

                var config = new CrsEndpointConfiguration( registry, model );

                var settings = applicationServices.GetService<JsonSerializerSettings>() ?? new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                };
                config.ChangeDefaultBinder( new JsonCommandBinder( settings ) );
                config.ChangeDefaultFormatter( new JsonResponseFormatter( settings ) );

                // Override configuration from the given lambda
                configuration?.Invoke( config );

                IEndpointModel endpointModel = config.Build( crsPath.Value );

                crsApp.Use<CrsOwinMiddleware>( applicationServices, endpointModel );
            } );
        }
    }
}
