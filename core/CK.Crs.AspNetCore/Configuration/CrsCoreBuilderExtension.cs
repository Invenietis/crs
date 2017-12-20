using CK.Crs;
using CK.Crs.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class CrsCoreBuilderExtension
    {
        public static ICrsCoreBuilder AddCrs( this IServiceCollection services, Action<ICommandRegistry> commandsConfigurationn )
        {
            var builder = services.AddCrsCore( commandsConfigurationn );
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

        public static IApplicationBuilder UseCrs( this IApplicationBuilder app, PathString crsPath, Action<ICrsEndpointConfiguration> configuration )
        {
            return app.Map( crsPath, crsApp =>
            {
                ICrsModel model = app.ApplicationServices.GetRequiredService<ICrsModel>();
                ICommandRegistry registry = app.ApplicationServices.GetRequiredService<ICommandRegistry>();

                var config = new CrsEndpointConfiguration( registry, model );
                config.ChangeDefaultBinder<JsonCommandBinder>();
                configuration?.Invoke( config );

                var endpointModel = config.Build( crsPath );

                var settings = ActivatorUtilities.GetServiceOrCreateInstance<JsonSerializerSettings>( app.ApplicationServices );
                IResponseFormatter responseFormatter = new JsonResponseFormatter( settings );
                crsApp.UseMiddleware<CrsMiddleware>( endpointModel, responseFormatter );
            } );
        }
    }
}
