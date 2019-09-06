using CK.Core;
using System;

namespace CK.Crs.Configuration
{
    public interface ICrsEndpointConfiguration
    {
        CKTraitContext TagContext { get; }
        ICrsEndpointConfiguration FilterCommands( Func<ICommandModel, bool> filter );
        ICrsEndpointConfiguration SkipAmbientValuesValidation();
        ICrsEndpointConfiguration SkipModelValidation();
        ICrsEndpointConfiguration ChangeDefaultBinder( ICommandBinder binder );
        ICrsEndpointConfiguration ChangeCallerIdName( string newCallerIdName );
        ICrsEndpointConfiguration ChangeDefaultFormatter( IResponseFormatter responseFormatter);
        ICrsEndpointConfiguration AddSecurityFilter<T>() where T : ICommandSecurityFilter;
    }

}
