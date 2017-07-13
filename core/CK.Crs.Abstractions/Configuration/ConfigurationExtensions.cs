using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class ConfigurationExtensions
    {
        public static ICrsEndpointConfigurationRoot AcceptAll( this ICrsEndpointConfiguration configuration )
        {
            return configuration.Apply( _ => true );
        }
    }
}
