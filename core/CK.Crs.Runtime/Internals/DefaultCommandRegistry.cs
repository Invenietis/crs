using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    class DefaultCommandRegistry : ICommandRegistry
    {
        Dictionary<CommandName, ICommandModel> Map { get; } = new Dictionary<CommandName, ICommandModel>();

        public DefaultCommandRegistry( CKTraitContext traitContext )
        {
            TraitContext = traitContext;
        }

        public IEnumerable<ICommandModel> Registration
        {
            get { return Map.Values; }
        }

        public CKTraitContext TraitContext { get; }

        public void Register( ICommandModel descriptor )
        {
            Map.Add( descriptor.Name, descriptor );
        }

        public ICommandModel GetCommandByName( CommandName name )
        {
            return Map.GetValueWithDefault( name, null );
        }
    }
}
