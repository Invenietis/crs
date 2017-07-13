using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{

    public interface IWebEventListeners
    {
        Task RegisterUniqueListener( string callerId, string eventName, CancellationToken token );
    }


    public interface ILiveEventModel
    {
        string Name { get; }

        string CallerId { get; }
    }

    public interface ILiveEventStore
    {
        Task RegisterListener( string clientId, string eventName );

        Task RemoveListener( string clientId, string eventName );

        Task<bool> IsRegistered( string clientId, string name );

        Task<IEnumerable<ILiveEventModel>> GetListeners( string clientId );
    }
}
