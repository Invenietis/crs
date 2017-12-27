using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// Fluent command registration and configuration
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    public interface ICommandConfiguration<TConfig>
        where TConfig : ICommandConfiguration<TConfig>
    {
        /// <summary>
        /// Overrides the name of the command.
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        ICommandConfiguration<TConfig> CommandName( string commandName );
        /// <summary>
        /// Overrides the name of the command.
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        ICommandConfiguration<TConfig> CustomBinder<T>() where T : ICommandBinder;

        /// <summary>
        /// Register a new command
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <returns></returns>
        ICommandRegistration Register<TCommand>() where TCommand: class;
    }

    /// <summary>
    /// Fluent command configuration
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    public interface ICommandConfigurationWithHandling<TConfig>
         where TConfig : ICommandConfiguration<TConfig>
    {
        /// <summary>
        /// Sets an handler for a command.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        ICommandConfiguration<TConfig> HandledBy<THandler>();
    }

    /// <summary>
    /// Fluent command configuration
    /// </summary>
    public interface ICommandRegistration : ICommandConfiguration<ICommandRegistration>, ICommandConfigurationWithHandling<ICommandRegistration>
    {
        /// <summary>
        /// Gets the <see cref="CommandModel"/>.
        /// </summary>
        ICommandModel Model { get; }
    }
}
