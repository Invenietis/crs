using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class CrsExecutionExtensions
    {
        public static void AddCommandExecutor( this IServiceCollection services )
        {
            services.AddSingleton<ICommandExecutionFactories, DefaultFactories>();
            services.AddSingleton<ICommandHandlerFactory, DefaultFactories>();
            services.AddSingleton<ICommandDecoratorFactory, DefaultFactories>();
            services.AddSingleton<IExternalComponentFactory, DefaultFactories>();
        }
    }
}
