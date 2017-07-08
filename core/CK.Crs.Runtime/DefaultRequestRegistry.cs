using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    public class DefaultRequestRegistry : IRequestRegistry
    {
        List<RequestDescription> Map { get; } = new List<RequestDescription>();

        public DefaultRequestRegistry(CKTraitContext traitContext)
        {
            TraitContext = traitContext;
        }

        public IEnumerable<RequestDescription> Registration
        {
            get { return Map; }
        }

        public CKTraitContext TraitContext { get; }

        public void Register( RequestDescription descriptor )
        {
            Map.Add( descriptor );
        }
    }
}