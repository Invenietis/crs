using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IActorIdProvider
    {
        /// <summary>
        /// Gets user identifier by name
        /// </summary>
        /// <param name="userName">A user name</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The user identifier (<see cref="int"/>).</returns>
        Task<int> GetUserIdAsync( string userName, CancellationToken cancellationToken = default( CancellationToken ) );
    }

}
