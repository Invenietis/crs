using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    public class CommandRegistry : ICommandRegistry
    {
        List<CommandDescription> Map { get; } = new List<CommandDescription>();

        public CommandRegistry(CKTraitContext traitContext)
        {
            TraitContext = traitContext;
        }

        public IEnumerable<CommandDescription> Registration
        {
            get { return Map; }
        }

        public CKTraitContext TraitContext { get; }

        public void Register( CommandDescription descriptor )
        {
            Map.Add( descriptor );
        }
    }
}