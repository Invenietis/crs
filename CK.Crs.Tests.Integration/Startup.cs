using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using CK.Crs.Handlers;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using System;

namespace CK.Crs.Tests.Integration
{
    public class Startup
    {
        public int IDatabase { get; private set; }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddCommandReceiver( options =>
            {
                options.EnableLongRunningCommands = false;
                options.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>().CommandName( "transfer" ).IsLongRunning();
                options.Register<WithdrawMoneyCommand, WithDrawyMoneyHandler>().CommandName( "withdraw" ).AddDecorator<TransactionAttribute>();
                options.Register<UserCommand, UserHandler>().CommandName( "addUser" ).AddDecorator<TransactionAttribute>();
            } );

            services.AddMvc();
            services.AddSingleton<IRepository<UserModel>, UserRepository>();
            services.AddTransient<UserHandler>();
        }

        public void Configure( IApplicationBuilder app )
        {
            app.UseStaticFiles();
            app.UseCommandReceiver( "/c/admin", options =>
            {
                options
                    .AddFilter<AmbientValuesFilter>()
                    .AddFilter<HttpsRequiredFilter>()
                    .AddFilter<CK.Crs.Extensions.AuthorizationFilter>()
                    .AddCommands(
                        registry => registry.Registration.Where( c => c.CommandType.Namespace.StartsWith( "CK.Crs" ) ),
                        config => config.AddPermission( CK.Authorization.MinGrantLevel.Administrator ) )
                    .AddCommand<TransferAmountCommand>().CommandName( "transfer" ).AddDecorator<TransactionAttribute>();
            } );
            app.UseCommandReceiver( "/c/public", options =>
            {
            } );

            //options.OnCommandEvent( x => Azure.Publish );
            app.UseMvc();

        }
    }
}
