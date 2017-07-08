using CK.Core;
using System.Collections.Generic;

namespace CK.Crs
{

    /// <summary>
    /// Defines a command registry, which commands are globaly stored.
    /// </summary>
    public interface IRequestRegistry : ITraitContextProvider
    {
        void Register( RequestDescription descriptor );

        IEnumerable<RequestDescription> Registration { get; }
    }
}
