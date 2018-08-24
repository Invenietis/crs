using CK.Crs;
using CK.Crs.Configuration;
using CK.Crs.Results;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsConfigurationExtensions
    {
        public static ICrsCoreBuilder AddCrsCore<T>( this IServiceCollection services, Action<ICommandRegistry> registry ) where T : class, ICommandHandlerActivator
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

            services.AddSingleton<ICommandHandlerActivator, T>();
            services.AddSingleton<ICommandHandlerFactory, DefaultCommandHandlerFactory>();
            services.AddSingleton<ICommandHandlerInvoker, DefaultCommandInvoker>();
            services.AddSingleton<ITypedCommandHandlerInvoker, TypedCommandInvoker>();
            services.AddSingleton<IResultDispatcherSelector, DefaultResutDispatcherSelector>();
            services.AddSingleton<IResultReceiverProvider, DefaultResultReceiverProvider>();

            services.AddSingleton<ICrsConnectionManager, DefaultConnectionManager>();
            services.AddSingleton( builder.Registry );

            var crsBuilder = new CrsCoreBuilder( builder );
            services.AddSingleton<ICrsCoreBuilder>( crsBuilder );
            return crsBuilder;
        }
        /// <summary>
        /// Adds crs runtime services and returns a <see cref="ICrsCoreBuilder"/> for registering additionnal components.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICrsCoreBuilder AddCrsCore( this IServiceCollection services, Action<ICommandRegistry> registry )
        {
            return AddCrsCore<DefaultHandlerActivator>( services, registry );
        }
    }
}
