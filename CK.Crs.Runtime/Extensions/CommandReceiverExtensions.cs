using System;
using CK.Crs.Runtime;
using CK.Crs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommandReceiverExtensions
    {
        /// <summary>
        /// Registers commands into the <see cref="ICommandRegistry"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommandRegistration( this IServiceCollection services, Action<ICommandRegistry> configuration )
        {
            var r =  new CommandRegistry();

            configuration(r);

            services.AddSingleton<ICommandRegistry>( r );

            return services;
        }
    }
}
