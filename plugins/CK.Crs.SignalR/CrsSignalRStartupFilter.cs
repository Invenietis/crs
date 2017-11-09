using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Builder;

namespace CK.Crs.SignalR
{
    class CrsSignalRStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure( Action<IApplicationBuilder> next )
        {
            return app =>
            {
                app.UseSignalR( r => r.MapHub<CrsHub>( "crs" ) );
                next( app );
            };
        }
    }
}
