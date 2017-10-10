using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Main entry point for receiving commands sent by a client from Http.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHttpCommandReceiver<T>
    {
        /// <summary>
        /// Receives a command from a caller.
        /// </summary>
        /// <param name="command">The received command.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<Response> ReceiveCommand( T command, ICommandContext context );
    }
}
