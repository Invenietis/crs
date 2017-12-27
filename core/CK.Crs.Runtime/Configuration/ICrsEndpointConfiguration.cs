using CK.Core;
using System;

namespace CK.Crs.Configuration
{
    public interface ICrsEndpointConfiguration
    {
        CKTraitContext TraitContext { get; }
        ICrsEndpointConfiguration FilterCommands( Func<ICommandModel, bool> filter );
        ICrsEndpointConfiguration SkipAmbientValuesValidation();
        ICrsEndpointConfiguration SkipModelValidation();
        ICrsEndpointConfiguration ChangeDefaultBinder<T>() where T : ICommandBinder;
        ICrsEndpointConfiguration ChangeCallerIdName( string newCallerIdName );
        ICrsEndpointConfiguration ChangeDefaultFormatter( IResponseFormatter responseFormatter);
        ICrsEndpointConfiguration AddSecurityFilter<T>() where T : ICommandSecurityFilter;
    }

}
