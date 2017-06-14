using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Main option root for global CRS configuration
    /// </summary>
    public class CommandReceiverOption
    {
        public CommandReceiverOption( IServiceCollection services, ICommandRegistry r, IAmbientValuesRegistration a )
        {
            Services = services;
            Commands = r;
            AmbientValues = a;
        }

        public ICommandRegistry Commands { get; }

        public IServiceCollection Services { get; }

        public IAmbientValuesRegistration AmbientValues { get; }
    }
}
