using System;
using System.Collections.Generic;

namespace CK.Crs.CommandDiscoverer
{
    internal readonly struct HandlerWithGenerics
    {
        private Type Handler { get; }
        private IEnumerable<Type[]> Interfaces { get; }

        public HandlerWithGenerics( Type handler, IEnumerable<Type[]> interfaces )
        {
            Interfaces = interfaces;
            Handler = handler;
        }

        public void Deconstruct( out Type handler, out IEnumerable<Type[]> interfaces )
        {
            handler = Handler;
            interfaces = Interfaces;
        }
    }
}
