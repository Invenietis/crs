using CK.Crs.Runtime;

namespace CK.Crs.Scalability
{
    public interface ICommandReceiverConfigurationOutput
    {
        /// <summary>
        /// Adds a command receiver.
        /// </summary>
        /// <param name="dispatcher"></param>
        void AddDispatcher(ICommandResponseDispatcher dispatcher);
    }
}