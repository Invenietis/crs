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
        readonly CKTraitContext _traitContext;

        public CrsConfigurationBuilder( IServiceCollection services, string traitContextName = "Crs" )
        {
            _services = services;
            _traitContext = new CKTraitContext( traitContextName );
            _model = new CrsModel( _traitContext );
            _commands = new DefaultCommandRegistry( _traitContext );
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
