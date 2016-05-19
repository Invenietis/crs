using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class CommandExecutorExtensions
    {
        public static void AddCommandExecutor( this IServiceCollection services )
        {
            services.AddScoped<ICommandHandlerFactory, DefaultFactory>();
        }
    }
}
