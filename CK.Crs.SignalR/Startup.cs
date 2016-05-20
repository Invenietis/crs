using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Owin;

namespace CK.Crs
{
    public static class SignalRExtensions
    {
        public static void AddCrsSignalR( this IServiceCollection services )
        {
            services.AddTransient<CrsHub>();
        }

        public static void MapCrsSignalR( this IAppBuilder app, IServiceProvider services )
        {
            var resolver = new CustomSignalRDependencyResolver( services );

            app.MapSignalR( new HubConfiguration
            {
                EnableJSONP = true,
                EnableJavaScriptProxies = false,
                EnableDetailedErrors = true,
                Resolver = resolver
            } );
            GlobalHost.DependencyResolver = resolver;
        }

        public class CustomSignalRDependencyResolver : DefaultDependencyResolver
        {
            private readonly IServiceProvider _serviceProvider;

            public CustomSignalRDependencyResolver( IServiceProvider serviceProvider )
            {
                _serviceProvider = serviceProvider;
            }

            public override object GetService( Type serviceType )
            {
                var service = _serviceProvider.GetService(serviceType);

                return service ?? base.GetService( serviceType );
            }

            public override IEnumerable<object> GetServices( Type serviceType )
            {
                var services = _serviceProvider.GetServices(serviceType);

                return services.Concat( base.GetServices( serviceType ) );
            }

        }
    }
}
