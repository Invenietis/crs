namespace CK.Infrastructure.Commands
{
    public interface ICommandExecutorSelector
    {
        /// <summary>
        /// Selects the best <see cref="ICommandExecutor"/> implementation regarding <see cref="CommandContext"/>
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        ICommandExecutor SelectExecutor( CommandContext commandContext );
    }
}
