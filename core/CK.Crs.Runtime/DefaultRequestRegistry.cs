using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    public class DefaultRequestRegistry : ICommandRegistry
    {
        Dictionary<CommandName,CommandModel> Map { get; } = new Dictionary<CommandName,CommandModel>();

        public DefaultRequestRegistry( CKTraitContext traitContext )
        {
            TraitContext = traitContext;
        }

        public IEnumerable<CommandModel> Registration
        {
            get { return Map.Values; }
        }

        public CKTraitContext TraitContext { get; }

        public void Register( CommandModel descriptor )
        {
            Map.Add( descriptor.Name, descriptor );
        }

        public CommandModel GetCommandByName( CommandName name )
        {
            return Map.GetValueWithDefault( name, null );
        }
    }
}
