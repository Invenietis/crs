using CK.Crs;
using CK.Crs.Configuration;
using CK.Crs.Results;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsConfigurationExtensions
    {
        public static ICrsCoreBuilder AddCrsCore( this IServiceCollection services, Action<ICommandRegistry> commandsConfigurationn )
        {
            CrsConfigurationBuilder builder = new CrsConfigurationBuilder( services );
            commandsConfigurationn( builder.Registry );

            services.AddSingleton<ICommandHandlerFactory, DefaultCommandHandlerFactory>();
            services.AddSingleton<ICommandHandlerInvoker, DefaultCommandInvoker>();
            services.AddSingleton<ITypedCommandHandlerInvoker, TypedCommandInvoker>();
            services.AddSingleton<IResultDispatcherSelector, DefaultResutDispatcherSelector>();
            services.AddSingleton<IResultReceiverProvider, DefaultResultReceiverProvider>();
            services.AddSingleton<ICrsConnectionManager, DefaultConnectionManager>();

            services.AddSingleton( builder.Registry );

            return new CrsCoreBuilder( builder );
        }
    }
}
