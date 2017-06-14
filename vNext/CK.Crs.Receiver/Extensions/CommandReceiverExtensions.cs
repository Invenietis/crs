using System;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Filtering;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.DependencyInjection;
using CK.Crs;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommandReceiverExtensions
    {
        public static IServiceCollection AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOption> configuration )
        {
            services.AddMemoryCache();
            services.AddSingleton<ICommandFilterFactory, DefaultCommandFilterFactory>();
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();

            services.AddScoped<IAmbientValues, AmbientValues>();

            var r =  new CommandRegistry();
            var a = new AmbientValuesRegistration( services );
            var o = new CommandReceiverOption(services, r, a);

            configuration(o);

            services.AddSingleton<ICommandRegistry>( r );
            services.AddSingleton<IAmbientValuesRegistration>( a );

            return services;
        }
    }
}
