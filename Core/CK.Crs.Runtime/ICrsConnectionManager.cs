using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICrsConnectionManager
    {
        /// <summary>
        /// Registers the <see cref="CallerId"/>
        /// </summary>
        /// <param name="correlation"></param>
        /// <returns></returns>
        Task AddConnection( CallerId correlation );

        /// <summary>
        /// This is called when a <see cref="CallerId"/> has been registered
        /// </summary>
        event Func<CallerId, Task> ConnectionAdded;

        Task RemoveConnection( CallerId correlation );
        event Func<CallerId, Task> ConnectionRemoved;
    }

}
