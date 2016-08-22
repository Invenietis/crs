using System;
using CK.Core;
using CK.Crs.Runtime.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class CommandExecutorExtensions
    {
        public static void AddCommandExecutor( this IServiceCollection services, Action<CrsExecutorConfiguration> configuration = null )
        {
            // The execution factory is scoped to the execution of the command.
            services.AddScoped<IExecutionFactory, DefaultExecutionFactory>();

            CrsExecutorConfiguration config = new CrsExecutorConfiguration();
            configuration?.Invoke( config );

            services.AddSingleton( typeof( ICommandRunningStore ), config.CommandRunningStore );
        }
    }
}
