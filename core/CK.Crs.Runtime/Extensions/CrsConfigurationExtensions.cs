using CK.Crs;
using CK.Crs.Configuration;
using CK.Crs.Results;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsConfigurationExtensions
    {
        /// <summary>
        /// Adds crs runtime services and returns a <see cref="ICrsCoreBuilder"/> for registering additionnal components.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICrsCoreBuilder AddCrsCore( this IServiceCollection services, Action<ICommandRegistry> registry )
        {
            if( services == null )
            {
                throw new ArgumentNullException( nameof( services ) );
            }

            if( registry == null )
            {
                throw new ArgumentNullException( nameof( registry ) );
            }

            var builderDescriptor = services.SingleOrDefault( x => x.ServiceType == typeof( ICrsCoreBuilder ) );
            if( builderDescriptor != null )
            {
                var impl = (ICrsCoreBuilder)builderDescriptor.ImplementationInstance;
                registry( impl.Registry );
                return impl;
            }

            var builder = new CrsConfigurationBuilder( services );
            registry( builder.Registry );

            services.AddScoped<ICommandHandlerFactory, DefaultCommandHandlerFactory>();
            services.AddScoped<ICommandHandlerInvoker, DefaultCommandInvoker>();
            services.AddScoped<ITypedCommandHandlerInvoker, TypedCommandInvoker>();
            services.AddScoped<IResultDispatcherSelector, DefaultResutDispatcherSelector>();
            services.AddScoped<IResultReceiverProvider, DefaultResultReceiverProvider>();

            services.AddSingleton<ICrsConnectionManager, DefaultConnectionManager>();
            services.AddSingleton( builder.Registry );

            var crsBuilder = new CrsCoreBuilder( builder );
            services.AddSingleton<ICrsCoreBuilder>( crsBuilder );
            return crsBuilder;
        }
    }
}
