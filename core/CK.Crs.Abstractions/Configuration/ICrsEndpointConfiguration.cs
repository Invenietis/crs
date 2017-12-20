using CK.Core;
using System;

namespace CK.Crs
{
    public interface ICrsEndpointConfiguration
    {
        CKTraitContext TraitContext { get; }
        ICrsEndpointConfiguration FilterCommands( Func<CommandModel, bool> filter );
        ICrsEndpointConfiguration SkipAmbientValuesValidation();
        ICrsEndpointConfiguration SkipModelValidation();
        ICrsEndpointConfiguration ChangeDefaultBinder<T>() where T : ICommandBinder;
        ICrsEndpointConfiguration ChangeCallerIdName( string newCallerIdName );
    }

}
