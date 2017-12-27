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

        /// <summary>
        /// Adds a new <see cref="CommandModel"/> to the registry
        /// </summary>
        /// <param name="command"></param>
        void Register( ICommandModel command );

        ICommandRegistration Register<TCommand>() where TCommand : class;

        /// <summary>
        /// Gets all the available <see cref="CommandModel"/> in the registry
        /// </summary>
        IEnumerable<ICommandModel> Registration { get; }

        /// <summary>
        /// Gets a <see cref="CommandModel"/> from a <see cref="CommandName"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ICommandModel GetCommandByName( CommandName name );
    }
}
