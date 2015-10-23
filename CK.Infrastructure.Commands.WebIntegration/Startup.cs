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
            services.AddCommandReceiver();
        }

        public void Configure( IApplicationBuilder app )
        {
            app.UseIISPlatformHandler();
            app.UseStaticFiles();
            app.UseCommandReceiver( "c", x =>
            {
                x.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>( route: "TransferAmount", isLongRunning: true );
            } );

            app.UseSignalR();
        }
    }
}
