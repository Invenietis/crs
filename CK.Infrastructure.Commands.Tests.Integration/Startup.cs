using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Handlers;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Infrastructure.Commands.WebIntegration
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddCommandReceiver( options =>
            {
                options.EnableLongRunningSupport = false;
                // TODO: when this option is enabled, 
                // do we need to check that SignalR (or other long running stuff) is avalable and correctly setup?
                options
                    .Register<TransferAmountCommand, TransferAlwaysSuccessHandler>( route: "/c/v1/TransferAmount", isLongRunning: true )
                    .Register<WithdrawMoneyCommand, WithDrawyMoneyHandler>( "/c/labs/WithdrawMoney", false );
            } );
        }

        public void Configure( IApplicationBuilder app )
        {
            app.UseStaticFiles();
            app.UseCommandReceiver( "/c/v1" );
            app.UseCommandReceiver( "/c/labs" );
        }
    }
}
