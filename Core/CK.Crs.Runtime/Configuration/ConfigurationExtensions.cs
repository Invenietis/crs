using CK.Crs.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// The endpoint will accept all commands defined in the command registry.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ICrsEndpointConfiguration AcceptAll( this ICrsEndpointConfiguration configuration )
        {
            return configuration.FilterCommands( _ => true );
        }
    }
}
