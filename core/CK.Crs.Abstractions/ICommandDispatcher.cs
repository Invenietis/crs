using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Dispatch a command on a local or distributed channel depending of implementations.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Sends the command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command to send.</param>
        /// <param name="context">The related <see cref="ICommandContext"/>.</param>
        /// <returns></returns>
        Task SendAsync<T>(T command, ICommandContext context) where T : class;
    }
}
