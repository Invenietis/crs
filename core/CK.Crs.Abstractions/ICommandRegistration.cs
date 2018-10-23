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
        /// Register a new command
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        ICommandRegistration Register<TCommand>() where TCommand: class;

        /// <summary>
        /// Sets an handler for a command.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        ICommandRegistration HandledBy<THandler>();

        /// <summary>
        /// Gets the <see cref="CommandModel"/>.
        /// </summary>
        ICommandModel Model { get; }
    }
}
