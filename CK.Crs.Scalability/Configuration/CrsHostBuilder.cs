using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Scalability.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace CK.Crs.Scalability
{
    public class CrsHostBuilder
    {
        Type _startupT;
        IServiceCollection _services;
        IActivityMonitor _monitor;

        public CrsHostBuilder UseActivityMonitor(IActivityMonitor monitor)
        {
            _monitor = monitor;
            return this;
        }


        public CrsHostBuilder UserServices( IServiceCollection services )
        {
            _services = services;
            return this;
        }

        public CrsHostBuilder UseStartup<T>() where T : ICrsStartup
        {
            _startupT = typeof(T);
            return this;
        }

        public ICrsHost Build()
        {
            var startup = (ICrsStartup)Activator.CreateInstance(_startupT);

            if (_services == null) _services = new ServiceCollection();
            startup.ConfigureServices(_services);

            if (_monitor == null) _monitor = new ActivityMonitor();

            var services = _services.BuildServiceProvider();

            var app = new DefaultCrsBuilder( services );
            startup.Configure( app );

            var crsConfiguration = new CrsConfiguration(String.Empty, services.GetRequiredService<ICommandRegistry>());
            var handlerBuilder = new CrsHostHandlerBuilder( crsConfiguration );
            handlerBuilder.ApplyDefaultConfigurationOrConfigure();

            startup.ConfigureCrs( crsConfiguration );

            var handler = handlerBuilder.Build( services );
            return new CrsHost(_monitor, handler, services, app.Listeners, app.Dispatchers);
        }

        class CrsHostHandlerBuilder : CrsHandlerBuilder<CrsConfiguration>
        {
            public CrsHostHandlerBuilder( CrsConfiguration config )
            {
                AddConfiguration(config);
            }
            protected override void ConfigureDefault( CrsConfiguration configuration)
            {
                configuration.Pipeline
                   .Clear()
                   .Use<DirectCommandRouterComponent>()
                   .UseJsonCommandBuilder()
                   .UseDefaultCommandExecutor()
                   .UseJsonCommandWriter();
            }
        }
    }


}
