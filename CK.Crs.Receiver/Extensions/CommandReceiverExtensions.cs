﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Filtering;
using CK.Crs.Runtime.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class CommandReceiverExtensions
    {
        public static void AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOption> registration )
        {
            services.AddMemoryCache();
            services.AddScoped<ICommandFilterFactory, DefaultCommandFilterFactory>();
            services.AddScoped<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();
            services.AddScoped<IAmbientValues, AmbientValues>();

            var r =  new CommandRegistry( services );
            var a = new AmbientValuesRegistration( services );
            var o = new CommandReceiverOption(r, a);

            registration( o );

            services.AddSingleton<ICommandRegistry>( r );
            services.AddSingleton<IAmbientValuesRegistration>( a );
        }
    }
}
