using CK.Core;
using System.Collections.Generic;

namespace CK.Crs
{

    /// <summary>
    /// Defines a command registry, which commands are globaly stored.
    /// </summary>
    public interface ICommandRegistry
    {
        /// <summary>
        /// Gets the CRS <see cref="CKTraitContext"/>.
        /// </summary>
        CKTraitContext TraitContext { get; }

        void Register( CommandModel command );

        IEnumerable<CommandModel> Registration { get; }

        CommandModel GetCommandByName( CommandName name );
    }
}
