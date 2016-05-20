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
    public class CrsSignalRConfiguration
    {
        public string RunningCommandStoreImplementation { get; set; }
    }
    public static class SignalRExtensions
    {
        public static void AddCrsSignalR( this IServiceCollection services, Action<CrsSignalRConfiguration> configuration = null )
        {
            CrsSignalRConfiguration config = new CrsSignalRConfiguration();
            if( configuration != null ) configuration( config );

            services.AddSingleton<ICommandResponseDispatcher, CrsCommandDispatcher>();
            if( String.IsNullOrEmpty( config.RunningCommandStoreImplementation ) )
            {
                services.AddSingleton<ICommandRunningStore, InMemoryCommandRunningStore>();
            }
            else
            {
                services.AddSingleton( typeof( ICommandRunningStore ), CK.Core.SimpleTypeFinder.WeakResolver( config.RunningCommandStoreImplementation, true ) );
            }
        }
    }

    public class Startup
    {
        public static void ConfigureSignalR( IAppBuilder app, IServiceProvider services )
        {
            var resolver = new DependencyResolver( services );

            app.MapSignalR( new HubConfiguration
            {
                EnableJSONP = true,
                EnableJavaScriptProxies = false,
                EnableDetailedErrors = true,
                Resolver = resolver
            } );
        }

        class DependencyResolver : DefaultDependencyResolver
        {
            IServiceProvider _serviceProvider;
            public DependencyResolver( IServiceProvider serviceProvider )
            {
                _serviceProvider = serviceProvider;
            }

            public override object GetService( Type serviceType )
            {
                return _serviceProvider.GetService( serviceType ) ?? base.GetService( serviceType );
            }

            public override IEnumerable<object> GetServices( Type serviceType )
            {
                return _serviceProvider.GetServices( serviceType ) ?? base.GetServices( serviceType );
            }
        }
    }
}
