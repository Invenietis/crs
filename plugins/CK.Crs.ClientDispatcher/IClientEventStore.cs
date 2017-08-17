using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IClientEventStore
    {
        Task AddEventFilter( string clientId, string eventName );

        Task RemoveEventFilter( string clientId, string eventName );

        Task<bool> HasFilter( string clientId, string name );

        Task<IEnumerable<IEventFilter>> GetEventFilters( string clientId );
    }
}
