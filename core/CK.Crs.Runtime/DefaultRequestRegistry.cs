using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    public class DefaultRequestRegistry : ICommandRegistry
    {
        List<CommandModel> Map { get; } = new List<CommandModel>();

        public DefaultRequestRegistry(CKTraitContext traitContext)
        {
            TraitContext = traitContext;
        }

        public IEnumerable<CommandModel> Registration
        {
            get { return Map; }
        }

        public CKTraitContext TraitContext { get; }

        public void Register( CommandModel descriptor )
        {
            Map.Add( descriptor );
        }
    }
}
