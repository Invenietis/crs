using System;
using CK.Crs.Runtime;
using CK.Crs;
using CK.Core;

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
        public static IServiceCollection AddCommandRegistration( this IServiceCollection services, Action<ICommandRegistry> configuration, CKTraitContext existingTraitContext = null )
        {
            var r =  new CommandRegistry(existingTraitContext ?? new CKTraitContext("Crs"));

            configuration(r);

            services.AddSingleton<ICommandRegistry>( r );

            return services;
        }
    }
}
