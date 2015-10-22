using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Handlers;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;

namespace CK.Infrastructure.Commands.WebIntegration
{
    public class Startup
    {
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
        }

        public void Configure( IApplicationBuilder app )
        {
            app.UseCommandReceiver( "c", x =>
            {
                x.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>( route: "TransferAmount", isLongRunning: true );
            } );
            app.Run( async ( context ) =>
             {
                 await context.Response.WriteAsync( "Hello World!" );
             } );
        }
    }
}
