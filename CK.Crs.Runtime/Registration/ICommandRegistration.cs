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
        /// Add a <see cref="ICommandDecorator"/> to this command that will be invoked during command handling.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICommandConfiguration<TConfig> AddDecorator<T>() where T : ICommandDecorator;

        /// <summary>
        /// Overrides the name of the command.
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        ICommandConfiguration<TConfig> CommandName( string commandName );

        /// <summary>
        /// Adds extra data to the command description
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ICommandConfiguration<TConfig> AddExtraData( string key, object data );

        /// <summary>
        /// This command should be handled by an asynchronous executor and supports by the way deferred execution.
        /// </summary>
        /// <returns></returns>
        ICommandConfiguration<TConfig> IsAsync();
    }

    /// <summary>
    /// Fluent command configuration
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    public interface ICommandConfigurationWithHandling<TConfig>
    {
        /// <summary>
        /// Sets an handler for a command.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <returns></returns>
        ICommandConfigurationWithHandling<TConfig> HandledBy<THandler>() where THandler : ICommandHandler;
    }

    /// <summary>
    /// Fluent command configuration
    /// </summary>
    /// <typeparam name="TConfig"></typeparam>
    public interface ICommandConfigurationWithFilter<TConfig>
    {
        /// <summary>
        /// Add a <see cref="ICommandFilter"/> to this command that will be invoked as soon as the command is received by the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICommandConfigurationWithFilter<TConfig> AddFilter<T>() where T : ICommandFilter;
    }

    /// <summary>
    /// Fluent command configuration
    /// </summary>
    public interface ICommandRegistration : ICommandConfiguration<ICommandRegistration>, ICommandConfigurationWithHandling<ICommandRegistration>
    {
    }

    /// <summary>
    /// Fluent command configuration
    /// </summary>
    public interface ICommandRegistrationWithFilter : ICommandConfiguration<ICommandRegistrationWithFilter>, ICommandConfigurationWithFilter<ICommandRegistrationWithFilter>
    {
    }
}