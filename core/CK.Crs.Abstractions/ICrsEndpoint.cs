using CK.Core;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Main entry point for receiving commands sent by a client.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrsEndpoint<T> where T : class
    {
        /// <summary>
        /// Receives a command from a caller.
        /// </summary>
        /// <param name="command">The received command.</param>
        /// <param name="monitor">An <see cref="IActivityMonitor"/> used to monitor this command.</param>
        /// <param name="callerId">Identifies the caller</param>
        /// <returns></returns>
        Task<Response> ReceiveCommand(T command, IActivityMonitor monitor, string callerId);
    }
}
