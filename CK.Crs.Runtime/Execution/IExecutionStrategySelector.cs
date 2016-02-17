namespace CK.Crs.Runtime
{
    /// <summary>
    /// Implementations should uses the <see cref="CommandContext"/> informations to select the appropriate execution strategy to execute the command.
    /// </summary>
    public interface IExecutionStrategySelector
    {
        /// <summary>
        /// Selects the <see cref="IExecutionStrategy"/> that will be used to execute the command.
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        IExecutionStrategy SelectExecutionStrategy( CommandContext commandContext );
    }
}
