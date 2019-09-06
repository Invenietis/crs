using CK.Core;
using System;
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
        CKTraitContext TagContext { get; }

        /// <summary>
        /// Adds a new command <see cref="Type"/> to the registry
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="resultType"></param>
        /// <returns></returns>
        ICommandRegistration Register( Type commandType, Type resultType = null );

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
