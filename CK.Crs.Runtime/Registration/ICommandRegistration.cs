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

        ICommandConfiguration<TConfig> AddExtraData( string key, object data );

        ICommandConfiguration<TConfig> IsAsync();
    }

    public interface ICommandConfigurationWithHandling<TConfig>
    {
        ICommandConfigurationWithHandling<TConfig> HandledBy<THandler>() where THandler : ICommandHandler;
    }

    public interface ICommandConfigurationWithFilter<TConfig>
    {
        /// <summary>
        /// Add a <see cref="ICommandFilter"/> to this command that will be invoked as soon as the command is received by the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICommandConfigurationWithFilter<TConfig> AddFilter<T>() where T : ICommandFilter;
    }

    public interface ICommandRegistration : ICommandConfiguration<ICommandRegistration>, ICommandConfigurationWithHandling<ICommandRegistration>
    {
    }

    public interface ICommandRegistrationWithFilter : ICommandConfiguration<ICommandRegistrationWithFilter>, ICommandConfigurationWithFilter<ICommandRegistrationWithFilter>
    {
    }
}