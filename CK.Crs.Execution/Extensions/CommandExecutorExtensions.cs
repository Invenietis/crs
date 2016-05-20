using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public class CrsExecutorConfiguration
    {
        public string RunningCommandStoreImplementation { get; set; }
    }

    public static class CommandExecutorExtensions
    {
        public static void AddCommandExecutor( this IServiceCollection services, Action<CrsExecutorConfiguration> configuration = null )
        {
            services.AddScoped<ICommandHandlerFactory, DefaultFactory>();

            CrsExecutorConfiguration config = new CrsExecutorConfiguration();
            configuration?.Invoke( config );

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
}
