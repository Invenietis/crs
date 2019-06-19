using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace CK.Crs.SignalR
{
    class CrsSignalRStartupFilter : IStartupFilter
    {
        private readonly IOptions<CrsSignalROptions> _options;

        public CrsSignalRStartupFilter( IOptions<CrsSignalROptions> options )
        {
            _options = options;
        }

        public Action<IApplicationBuilder> Configure( Action<IApplicationBuilder> next )
        {
            return app =>
            {
                next( app );
                app.UseSignalR( r => r.MapHub<CrsHub>( _options.Value.CrsHubPath ) );
            };
        }
    }
}
