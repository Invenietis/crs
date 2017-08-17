using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Dispatch a command on a local channel.
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// Sends the command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command to send.</param>
        /// <param name="context">The related <see cref="ICommandContext"/>.</param>
        /// <returns></returns>
        Task<Response> ReceiveCommand<T>( T command, ICommandContext context ) where T : class;
    }
}
