using System;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Filtering;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class CommandReceiverExtensions
    {
        public static CommandReceiverOption AddCommandReceiver( this IServiceCollection services )
        {
            services.AddMemoryCache();
            services.AddSingleton<ICommandFilterFactory, DefaultCommandFilterFactory>();
            services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();

            services.AddScoped<IAmbientValues, AmbientValues>();

            var r =  new CommandRegistry( services );
            var a = new AmbientValuesRegistration( services );
            var o = new CommandReceiverOption(r, a);

            services.AddSingleton<ICommandRegistry>( r );
            services.AddSingleton<IAmbientValuesRegistration>( a );
            return o;
        }
    }
}
