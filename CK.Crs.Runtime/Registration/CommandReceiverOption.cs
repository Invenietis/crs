﻿using CK.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    /// <summary>
    /// Main option root for global CRS configuration
    /// </summary>
    public class CommandReceiverOption
    {
        public CommandReceiverOption( IServiceCollection services, ICommandRegistry r, IAmbientValuesRegistration a, CKTraitContext traits )
        {
            Services = services;
            Commands = r;
            AmbientValues = a;
            Traits = traits;
        }

        public CKTraitContext Traits { get; }

        public ICommandRegistry Commands { get; }

        public IServiceCollection Services { get; }

        public IAmbientValuesRegistration AmbientValues { get; }
    }
}
