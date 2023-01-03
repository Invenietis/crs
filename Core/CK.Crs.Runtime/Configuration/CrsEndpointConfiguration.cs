using System;
using System.Collections.Generic;
using System.Linq;
using CK.Core;

namespace CK.Crs.Configuration
{
    public class CrsEndpointConfiguration : ICrsEndpointConfiguration
    {
        readonly ICommandRegistry _registry;
        readonly ICrsModel _model;

        private ISet<ICommandModel> _commands;
        private bool _validateAmbientValues = true;
        private bool _validateModel = true;
        private string _callerIdName = "CallerId";
        private ICommandBinder _binder;
        private IResponseFormatter _responseFormatter;
        private IList<Type> _filters;

        public CKTraitContext TagContext => _model.TagContext;

        public CrsEndpointConfiguration( ICommandRegistry registry, ICrsModel model )
        {
            _registry = registry ?? throw new ArgumentNullException( nameof( registry ), "You must first AddCommands before AddEndpoint." );
            _model = model;
        }

        public ICrsEndpointConfiguration FilterCommands( Func<ICommandModel, bool> filter )
        {
            _commands = new HashSet<ICommandModel>( _registry.Registration.Where( filter ), new CommandModelComparer() );
            return this;
        }

        public ICrsEndpointConfiguration ChangeDefaultBinder( ICommandBinder commandBinder )
        {
            _binder = commandBinder;
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

        public ICrsEndpointConfiguration AddSecurityFilter<T>() where T : ICommandSecurityFilter
        {
            if( _filters == null ) _filters = new List<Type>();
            _filters.Add( typeof( T ) );
            return this;
        }

        public ICrsEndpointConfiguration ChangeCallerIdName( string newCallerIdName )
        {
            _callerIdName = newCallerIdName ?? throw new ArgumentNullException( nameof( newCallerIdName ) );
            return this;
        }

        public IEndpointModel Build( string path )
        {
            var receiverModel = new CrsReceiverModel( path, _model, _binder, _commands ?? _registry.Registration )
            {
                CallerIdName = _callerIdName,
                ResponseFormatter = _responseFormatter
            };
            if( _validateAmbientValues ) receiverModel.AddFilter( typeof( AmbientValuesValidationFilter ) );
            if( _validateModel ) receiverModel.AddFilter( typeof( ModelValidationFilter ) );
            if( _filters != null ) foreach( var f in _filters ) receiverModel.AddFilter( f );
            return receiverModel;
        }

    }
}
