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
        public static CommandReceiverOption AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOption> configuration )
        {
            services.AddMemoryCache();
            services.AddSingleton<ICommandFilterFactory, DefaultCommandFilterFactory>();
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();

            services.AddScoped<IAmbientValues, AmbientValues>();

            CKTraitContext traitContext = new CKTraitContext("Crs");

            var r = services.AddCommandRegistration( _ => { }, traitContext);
            var a = new AmbientValuesRegistration(services);
            var o = new CommandReceiverOption(services, r, a, traitContext);
            configuration(o);
            services.AddSingleton<IAmbientValuesRegistration>(a);
            services.AddSingleton(o);

            return o;
        }
    }
}
