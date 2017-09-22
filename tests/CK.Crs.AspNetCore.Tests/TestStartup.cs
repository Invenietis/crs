using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CK.Core;
using System.IO;
using CK.Monitoring.Handlers;
using CK.Monitoring;
using Microsoft.Extensions.Configuration;
using CK.Crs.Tests;
using System;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace CK.Crs.AspNetCore.Tests
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup( IHostingEnvironment environment )
        {
            Config = new ConfigurationBuilder().SetBasePath( environment.ContentRootPath ).AddEnvironmentVariables().Build();
        }

        public void ConfigureServices( IServiceCollection services )
        {
            services.AddAmbientValues( MapFromBaseCommandType );
            services.AddCrs( c => c.Commands( Commands ).Endpoints( Endpoints ) );
        }

        public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }

        /// <summary>
        /// Maps all ambient values properties based on the <see cref="CommandBase"/> used as a base type from all our commands
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ambientValuesRegistration"></param>
        private void MapFromBaseCommandType( IAmbientValuesRegistration ambientValuesRegistration)
        {
            ambientValuesRegistration
                .AddAmbientValueProviderFrom<CommandBase>()
                    .Select( t => t.ActorId ).ProvidedBy<AlwaysTrusted>() ;
        }

        private void Endpoints( ICrsEndpointConfigurationRoot e ) => e.For( typeof( TestEndpoint<> ) ).AcceptAll();

        private void Commands( ICommandRegistry registry ) =>
            registry
                .Register<TransferAmountCommand, TransferAlwaysSuccessHandler>()
                .Register<WithdrawMoneyCommand, WithDrawyMoneyHandler>();

        class AlwaysTrusted : IAmbientValueProvider
        {
            public Task<IComparable> GetValueAsync( IAmbientValues values )
            {
                return Task.FromResult<IComparable>( 0 );
            }
        }
    }
}
