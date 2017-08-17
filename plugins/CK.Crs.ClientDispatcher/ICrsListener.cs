using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Crs
{

    public interface ICrsListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<IEnumerable<IEventFilter>> Listeners( string callerId );

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
