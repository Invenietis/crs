using CK.Crs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Module
    {
        /// <summary>
        /// Adds the <see cref="ICommandDispatcher"/> to the services.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ICrsCoreBuilder AddDispatcher( this ICrsCoreBuilder builder )
        {
            builder.Services.AddScoped<ICommandDispatcher, CrsCommandDispatcher>();

            return builder;
        }
    }
}
