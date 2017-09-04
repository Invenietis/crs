using CK.Core;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsConfigurationExtensions
    {
        public static IServiceCollection AddAmbientValues( this IServiceCollection services, Action<IAmbientValuesRegistration> ambientValuesConfiguration )
        {
            var config = new CrsAmbientValuesConfiguration( services );
            var registration = config.Configure();
            ambientValuesConfiguration( registration );
            return services;
        }
    }
}
