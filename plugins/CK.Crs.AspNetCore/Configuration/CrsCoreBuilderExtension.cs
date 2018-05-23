using CK.Crs;
using CK.Crs.AspNetCore;
using CK.Crs.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class CrsCoreBuilderExtension
    {
        /// <summary>
        /// Adds basic CRS services to the collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="commandsConfigurationn"></param>
        /// <returns></returns>
        public static ICrsCoreBuilder AddCrs( this IServiceCollection services, Action<ICommandRegistry> commandsConfigurationn )
        {
            var builder = services.AddCrsCore( commandsConfigurationn );
            return builder.AddAspNetCoreHosting();
        }


        public static ICrsCoreBuilder AddAspNetCoreHosting( this ICrsCoreBuilder builder )
        {
            builder.Services.AddSingleton<ICommandFilterProvider, FilterProvider>();
            builder.Services.AddSingleton<ICommandFilter, AmbientValuesValidationFilter>();
            builder.Services.AddSingleton<ICommandFilter, ModelValidationFilter>();
            builder.Services.AddSingleton<JsonCommandBinder>();
            builder.Services.AddSingleton( new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            } );
            return builder;
        }

        /// <summary>
        /// Maps a default CRS endpoint to the given <paramref name="crsPath"/> that accepts all comamnds, validate ambient values, and valide command model.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="crsPath"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCrs( this IApplicationBuilder app, PathString crsPath )
        {
            return app.UseCrs( crsPath, null );
        }

        /// <summary>
        /// Maps a CRS enndpoint to the given <paramref name="crsPath"/>.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="crsPath"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCrs( this IApplicationBuilder app, PathString crsPath, Action<ICrsEndpointConfiguration> configuration )
        {
            return app.Map( crsPath, crsApp =>
            {
                var registry = app.ApplicationServices.GetRequiredService<ICommandRegistry>();
                var model = app.ApplicationServices.GetRequiredService<ICrsModel>();
                var config = new CrsEndpointConfiguration( registry, model );

                var settings = ActivatorUtilities.GetServiceOrCreateInstance<JsonSerializerSettings>( app.ApplicationServices );
                config.ChangeDefaultFormatter( new JsonResponseFormatter( settings ) );
                config.ChangeDefaultBinder<JsonCommandBinder>();

                // Override configuration from the given lambda
                configuration?.Invoke( config );

                var endpointModel = config.Build( crsPath );
                model.AddEndpoint( endpointModel );

                crsApp.UseMiddleware<CrsMiddleware>( endpointModel );
            } );
        }
    }
}
