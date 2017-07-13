using CK.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IListenerContext
    {
        string ClientId { get; }

        IActivityMonitor Monitor { get; }

        /// <summary>
        /// Notify when an underlying component has cancel the execution of this command...
        /// </summary>
        CancellationToken Aborted { get; }
    }

    public interface ICrsListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<IEnumerable<ILiveEventModel>> Listeners( string callerId );

        /// <summary>
        /// Registers an event listener for the given caller.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task AddListener( string eventName, IListenerContext context );

        /// <summary>
        /// Removes an event listener for the given caller.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task RemoveListener( string eventName, IListenerContext context );
    }
}
