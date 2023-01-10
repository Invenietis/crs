using CK.Crs.AspNetCore;
using CK.Crs.CommandDiscoverer;
using CK.Crs.Samples.AspNetCoreBasicApp.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CK.Crs.Samples.AspNetCoreBasicApp
{
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddCors( o =>
            {
                o.AddDefaultPolicy( builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed( ( host ) => true )
                    .AllowCredentials()
                );
            } );
            services.AddAmbientValues( ( registration ) => { } );
            services.AddSignalR();

            services.AddCrsCore( ( registry ) =>
                {
                    registry.RegisterHandlers( typeof( CommandHandler ).Assembly );
                } )
                .AddBackgroundCommandJobHostedService()
                .AddDispatcher()
                .AddInMemoryReceiver()
                .AddSignalR( ( opts ) =>
                {
                    opts.CrsHubPath = "/hubs/crs";
                } );
        }

        public void Configure( IApplicationBuilder app, IHostingEnvironment env )
        {
            app.UseDeveloperExceptionPage();
            app.UseCors();
            app.UseCrs( "/api/crs", ( o ) =>
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
                };
                var formatter = new JsonResponseFormatter( settings );
                o.ChangeDefaultFormatter( formatter );
            } );
        }
    }
}
