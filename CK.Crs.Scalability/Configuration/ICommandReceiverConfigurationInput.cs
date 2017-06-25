namespace CK.Crs.Scalability
{
    /// <summary>
    /// Defines a command receiver input configuration.
    /// </summary>
    public interface ICommandReceiverConfigurationInput
    {
        /// <summary>
        /// Adds a command receiver.
        /// </summary>
        /// <param name="listener"></param>
        void AddListener(ICommandListener listener);
    }
}
