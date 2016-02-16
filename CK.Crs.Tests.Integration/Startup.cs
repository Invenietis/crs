using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using CK.Crs.Handlers;

namespace CK.Crs.Tests.Integration
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddCommandReceiver( options =>
            {
                options.EnableLongRunningCommands = false;
                options.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>().CommandName( "transfer" ).IsLongRunning();
                options.Register<WithdrawMoneyCommand, WithDrawyMoneyHandler>().CommandName( "withdraw" ).AddDecorator<TransactionAttribute>();
            } );
        }

        public void Configure( IApplicationBuilder app )
        {
            app.UseStaticFiles();
            app.UseCommandReceiver( "/c/admin", options =>
            {
                options
                    .AddFilter<ClaimsAuthenticationFilter>()
                    .AddFilter<HttpsRequiredFilter>()
                    .AddCommands( 
                        registry => registry.Registration.Where( c => c.CommandType.Namespace.StartsWith( "CK.Crs" ) ),
                         config => config.AddFilter<AuthorizedFilter>() )
                    .AddCommand<TransferAmountCommand>().CommandName( "transfer" ).AddDecorator<TransactionAttribute>();
            } );
            app.UseCommandReceiver( "/c/public", options =>
            {
            } );
        }
    }
}
