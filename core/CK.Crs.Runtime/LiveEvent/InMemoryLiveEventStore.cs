using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class InMemoryLiveEventStore : ILiveEventStore
    {
        readonly ConcurrentDictionary<string, ISet<string>> _store;
        readonly StringComparer _comparer;

        public InMemoryLiveEventStore( )
        {
            _store = new ConcurrentDictionary<string, ISet<string>>();
            _comparer = StringComparer.OrdinalIgnoreCase;
        }

        Task<IEnumerable<ILiveEventModel>> ILiveEventStore.GetListeners( string clientId )
        {
            if( _store.TryGetValue( clientId, out ISet<string> liveEvents ) )
            {
                return Task.FromResult( liveEvents.Select( s => new LiveEventModel( clientId, s ) ).Cast<ILiveEventModel>() );
            }

            return Task.FromResult( Enumerable.Empty<ILiveEventModel>() );
        }

        Task<bool> ILiveEventStore.IsRegistered( string clientId, string name )
        {
            if( _store.TryGetValue( clientId, out ISet<string> liveEvents ) )
            {
                return Task.FromResult( liveEvents.Contains( name ) );
            }
            return Task.FromResult( false );
        }

        Task ILiveEventStore.RegisterListener( string clientId, string name )
        {
            _store.AddOrUpdate(
                clientId,
                ( k ) => new HashSet<string>( _comparer ) { name },
                ( k, v ) =>
                {
                    lock( _store )
                    {
                        v.Add( name );
                        return v;
                    }
                } );

            return Task.CompletedTask;
        }

        Task ILiveEventStore.RemoveListener( string clientId, string name )
        {
            if( _store.TryGetValue( clientId, out ISet<string> events ) )
            {
                lock( _store )
                {
                    events.Remove( name );
                    if( events.Count == 0 ) _store.TryRemove( clientId, out events );
                }
            }
            return Task.CompletedTask;
        }


        struct LiveEventModel : ILiveEventModel
        {
            public LiveEventModel( string callerId, string name )
            {
                CallerId = callerId;
                Name = name;
            }

            public string Name { get; set; }

            public string CallerId { get; set; }
        }

        class LiveEventModelComparer : IEqualityComparer<ILiveEventModel>
        {
            public bool Equals( ILiveEventModel x, ILiveEventModel y )
            {
                return x.CallerId == y.CallerId && x.Name == y.Name;
            }

            public int GetHashCode( ILiveEventModel obj )
            {
                return obj.CallerId.GetHashCode() ^ obj.Name.GetHashCode();
            }
        }

    }
}
