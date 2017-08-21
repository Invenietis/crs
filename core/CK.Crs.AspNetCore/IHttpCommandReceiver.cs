using CK.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Main entry point for receiving commands sent by a client.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHttpCommandReceiver<T> where T : class
    {
        /// <summary>
        /// Receives a command from a caller.
        /// </summary>
        /// <param name="command">The received command.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<Response> ReceiveCommand( T command, IHttpCommandContext context );
    }

    public interface IHttpCommandContext : ICommandContext
    {
        HttpContext GetHttpContext();
    }
}
