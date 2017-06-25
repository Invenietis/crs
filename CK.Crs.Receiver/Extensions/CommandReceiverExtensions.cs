using System;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Filtering;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.DependencyInjection;
using CK.Crs;
using CK.Core;

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

            CKTraitContext traitContext = new CKTraitContext("Crs");

            return services.AddCommandRegistration( r =>
            {
                var a = new AmbientValuesRegistration(services);
                var o = new CommandReceiverOption(services, r, a, traitContext);

                configuration(o);
                services.AddSingleton<IAmbientValuesRegistration>(a);
                services.AddSingleton(o);
            }, traitContext);
        }
    }
}
