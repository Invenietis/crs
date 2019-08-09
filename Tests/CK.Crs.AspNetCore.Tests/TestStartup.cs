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
using Microsoft.Extensions.Hosting;
using System.Threading;
using CK.Crs.Hosting;

namespace CK.Crs.AspNetCore.Tests
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup( Microsoft.AspNetCore.Hosting.IHostingEnvironment environment )
        {
            Config = new ConfigurationBuilder().SetBasePath( environment.ContentRootPath ).AddEnvironmentVariables().Build();
        }

        public void ConfigureServices( IServiceCollection services )
        {
            services.AddAmbientValues( MapFromBaseCommandType );
            services
                .AddCrsCore( Commands ) //.Endpoints( Endpoints ) )
                .AddInMemoryReceiver( host =>
                {
                    services.AddHostedService<BackgroundCommandJobHostedService>();
                } )
                .AddSignalR();
        }

        public void Configure( IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            app.UseCrs( "/crs" );
        }

        /// <summary>
        /// Maps all ambient values properties based on the <see cref="CommandBase"/> used as a base type from all our commands
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ambientValuesRegistration"></param>
        private void MapFromBaseCommandType( IAmbientValuesRegistration ambientValuesRegistration )
        {
            ambientValuesRegistration
                .AddProviderFrom<CommandBase>()
                    .Select( t => t.ActorId ).ProvidedBy<AlwaysTrusted>();
        }

        private void Commands( ICommandRegistry registry )
        {
            registry.Register<WithdrawMoneyCommand>().HandledBy<WithDrawyMoneyHandler>().CommandName( "withdraw" );
            registry.Register<TransferAmountCommand>().FireAndForget().HandledBy<TransferAlwaysSuccessHandler>();
        }

        class AlwaysTrusted : IAmbientValueProvider
        {
            public Type ValueType => typeof( int );

            public Task<IComparable> GetValueAsync( IAmbientValues values )
            {
                return Task.FromResult<IComparable>( 0 );
            }
        }
    }
}