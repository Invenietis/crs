using CK.Crs.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class ConfigurationExtensions
    {
        public static ICrsEndpointConfiguration AcceptAll( this ICrsEndpointConfiguration configuration )
        {
            return configuration.FilterCommands( _ => true );
        }
    }
}
