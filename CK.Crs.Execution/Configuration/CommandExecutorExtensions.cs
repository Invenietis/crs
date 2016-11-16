using System;
using CK.Core;
using CK.Crs.Runtime.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class CommandExecutorExtensions
    {
        public static void AddCommandExecutor( this IServiceCollection services, Action<ExecutorOption> configuration = null )
        {
            // The execution factory is scoped to the execution of the command.
            services.AddScoped<IExecutionFactory, DefaultExecutionFactory>();

            ExecutorOption config = new ExecutorOption();
            configuration?.Invoke( config );

            services.AddSingleton( typeof( ICommandRunningStore ), config.CommandRunningStore );
        }
    }
}
