using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class InMemoryLiveEventStore : IClientEventStore
    {
        readonly ConcurrentDictionary<string, ISet<string>> _store;
        readonly StringComparer _comparer;

        public InMemoryLiveEventStore( )
        {
            _store = new ConcurrentDictionary<string, ISet<string>>();
            _comparer = StringComparer.OrdinalIgnoreCase;
        }

        Task<IEnumerable<IEventFilter>> IClientEventStore.GetEventFilters( string clientId )
        {
            if( _store.TryGetValue( clientId, out ISet<string> liveEvents ) )
            {
                return Task.FromResult( liveEvents.Select( s => new LiveEventModel( clientId, s ) ).Cast<IEventFilter>() );
            }

            return Task.FromResult( Enumerable.Empty<IEventFilter>() );
        }

        Task<bool> IClientEventStore.HasFilter( string clientId, string name )
        {
            if( _store.TryGetValue( clientId, out ISet<string> liveEvents ) )
            {
                return Task.FromResult( liveEvents.Contains( name ) );
            }
            return Task.FromResult( false );
        }

        Task IClientEventStore.AddEventFilter( string clientId, string name )
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

        Task IClientEventStore.RemoveEventFilter( string clientId, string name )
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


        struct LiveEventModel : IEventFilter
        {
            public LiveEventModel( string callerId, string name )
            {
                ClientId = callerId;
                Name = name;
            }

            public string Name { get; set; }

            public string ClientId { get; set; }
        }

        class LiveEventModelComparer : IEqualityComparer<IEventFilter>
        {
            public bool Equals( IEventFilter x, IEventFilter y )
            {
                return x.ClientId == y.ClientId && x.Name == y.Name;
            }

            public int GetHashCode( IEventFilter obj )
            {
                return obj.ClientId.GetHashCode() ^ obj.Name.GetHashCode();
            }
        }

    }
}
