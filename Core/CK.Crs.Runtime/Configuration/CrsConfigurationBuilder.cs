using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CK.Crs.Configuration
{
    class CrsConfigurationBuilder : ICrsConfiguration
    {
        ICommandRegistry _commands;

        readonly IServiceCollection _services;
        readonly CrsModel _model;
        readonly CKTraitContext _tagContext;

        public CrsConfigurationBuilder( IServiceCollection services, string tagContextName = "Crs" )
        {
            _services = services;
            _tagContext = CKTraitContext.Create( tagContextName );
            _model = new CrsModel( _tagContext );
            _commands = new DefaultCommandRegistry( _tagContext );
        }

        internal IServiceCollection Services => _services;
        internal ICommandRegistry Registry => _commands;

        ICrsConfiguration ICrsConfiguration.Commands( Action<ICommandRegistry> registryConfiguration )
        {
            registryConfiguration( _commands );
            return this;
        }

        public virtual ICrsModel BuildModel()
        {
            return _model;
        }

    }
}
