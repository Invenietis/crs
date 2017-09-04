using CK.Core;
using CK.Crs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsConfigurationExtensions
    {
        public static ICrsCoreBuilder AddCrsCore( this IServiceCollection services, Action<ICrsConfiguration> configuration )
        {
            CrsConfigurationBuilder builder = new CrsConfigurationBuilder( services );
            configuration( builder );

            services.AddSingleton<ICommandHandlerFactory, DefaultCommandHandlerFactory>();
            services.AddSingleton<ICommandHandlerInvoker, DefaultCommandInvoker>();
            services.AddSingleton<IResultStrategy, DefaultResultStrategy>();
            services.AddSingleton<IClientDispatcher, NoOpClientDispatcher>();
            services.AddSingleton( builder.Registry );

            return new CrsCoreBuilder( builder );
        }
    }
}
