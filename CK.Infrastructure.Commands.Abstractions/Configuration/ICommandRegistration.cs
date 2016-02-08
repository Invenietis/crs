namespace CK.Infrastructure.Commands
{
    public interface ICommandConfiguration<TConfig>: CK.Core.IFluentInterface
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
        /// Tells that the command is long running.
        /// </summary>
        /// <returns></returns>
        ICommandConfiguration<TConfig> IsLongRunning();
    }

    public interface ICommandConfigurationWithHandling<TConfig> : CK.Core.IFluentInterface
    {
        ICommandConfigurationWithHandling<TConfig> HandledBy<THandler>() where THandler : ICommandHandler;
    }

    public interface ICommandConfigurationWithFilter<TConfig> : CK.Core.IFluentInterface
    {
        /// <summary>
        /// Add a <see cref="ICommandFilter"/> to this command that will be invoked as soon as the command is received by the server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICommandConfigurationWithFilter<TConfig> AddFilter<T>() where T : ICommandFilter;
    }

    public interface ICommandRegistration : ICommandConfiguration<ICommandRegistration>, ICommandConfigurationWithHandling<ICommandRegistration>, CK.Core.IFluentInterface
    {
    }

    public interface ICommandRegistrationWithFilter : ICommandConfiguration<ICommandRegistrationWithFilter>, ICommandConfigurationWithFilter<ICommandRegistrationWithFilter>, CK.Core.IFluentInterface
    {
    }
}