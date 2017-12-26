using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CK.Core;

namespace CK.Crs.Configuration
{
    public class CrsEndpointConfiguration : ICrsEndpointConfiguration
    {
        readonly ICommandRegistry _registry;
        readonly ICrsModel _model;

        private ISet<CommandModel> _commands;
        private Type _binder;
        private bool _validateAmbientValues = true;
        private bool _validateModel = true;
        private string _callerIdName = "CallerId";
        private IResponseFormatter _responseFormatter;

        public CKTraitContext TraitContext => _model.TraitContext;

        public CrsEndpointConfiguration( ICommandRegistry registry, ICrsModel model )
        {
            _registry = registry ?? throw new ArgumentNullException( nameof( registry ), "You must first AddCommands before AddEndpoint." );
            _model = model;
        }

        public ICrsEndpointConfiguration FilterCommands( Func<CommandModel, bool> filter )
        {
            _commands = new HashSet<CommandModel>( _registry.Registration.Where( filter ), new CommandModelComparer() );
            return this;
        }

        public ICrsEndpointConfiguration ChangeDefaultBinder<T>() where T : ICommandBinder
        {
            _binder = typeof( T );
            return this;
        }

        public ICrsEndpointConfiguration ChangeDefaultFormatter( IResponseFormatter responseFormatter )
        {
            _responseFormatter = responseFormatter;
            return this;
        }

        public ICrsEndpointConfiguration SkipAmbientValuesValidation()
        {
            _validateAmbientValues = false;
            return this;
        }

        public ICrsEndpointConfiguration SkipModelValidation()
        {
            _validateModel = false;
            return this;
        }

        public ICrsEndpointConfiguration ChangeCallerIdName( string newCallerIdName )
        {
            if( newCallerIdName == null )
            {
                throw new ArgumentNullException( nameof( newCallerIdName ) );
            }

            _callerIdName = newCallerIdName;
            return this;
        }

        public IEndpointModel Build( string path )
        {
            var receiverModel = new CrsReceiverModel( path, _model, _binder, _commands ?? _registry.Registration )
            {
                ApplyAmbientValuesValidation = _validateAmbientValues,
                ApplyModelValidation = _validateModel,
                CallerIdName = _callerIdName,
                ResponseFormatter = _responseFormatter
            };
            return receiverModel;
        }
    }
}
