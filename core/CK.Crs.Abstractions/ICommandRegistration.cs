using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// Fluent command registration and configuration
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    public interface ICommandRegistration
    {
        /// <summary>
        /// Overrides the name of the command.
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        ICommandRegistration CommandName( string commandName );

        /// <summary>
        /// Changes the command binder for this command exclusively
        /// </summary>
        /// <param name="binder "></param>
        /// <returns></returns>
        ICommandRegistration CustomBinder( ICommandBinder binder );

        /// <summary>
        /// Add filters for a command.
        /// </summary>
        /// <param name="filters"></param>
        ICommandRegistration AddFilters( IEnumerable<Type> filters );

        /// <summary>
        /// Sets an handler for a command.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        ICommandRegistration Handler( Type handler );

        /// <summary>
        /// Gets the <see cref="CommandModel"/>.
        /// </summary>
        ICommandModel Model { get; }
    }
}
