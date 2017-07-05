using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public class CrsAmbientValuesConfiguration
    {
        IServiceCollection _services;
        ICrsConfiguration _configuration;

        public CrsAmbientValuesConfiguration(ICrsConfiguration configuration, IServiceCollection services)
        {
            _services = services;
            _configuration = configuration;
        }

        public IAmbientValuesRegistration Configure()
        {
            _services.AddSingleton<IAmbientValueProviderFactory, DefaultAmbientValueFactory>();
            _services.AddScoped<IAmbientValues, AmbientValues>();

            var a = new AmbientValuesRegistration(_services);
            _services.AddSingleton<IAmbientValuesRegistration>(a);
            return a;
        }

    }
}
