﻿using CK.Core;
using CK.Crs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsConfigurationExtensions
    {
        public static ICrsCoreBuilder AddCrsCore( this IServiceCollection services, Action<ICrsConfiguration> configuration )
        {
            CrsConfigurationBuilder builder = new CrsConfigurationBuilder( services );
            configuration( builder );

            return new CrsCoreBuilder( builder );
        }

        public static ICrsCoreBuilder AddAmbientValues( this ICrsCoreBuilder builder, Action<IAmbientValuesRegistration> ambientValuesConfiguration )
        {
            var config = new CrsAmbientValuesConfiguration( builder.Services );
            var registration = config.Configure();
            ambientValuesConfiguration( registration );
            return builder;
        }
    }
}