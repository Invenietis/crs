using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    public interface ICommandRunningStore
    {
        Task AddCommandAsync( string callbackId, Guid commandId );

        Task RemoveCommandAsync( string callbackId, Guid commandId );

        Task<IReadOnlyCollection<Guid>> GetRunningCommands( string callbackId );
    }

    class InMemoryCommandRunningStore : ICommandRunningStore
    {
        public static ConcurrentDictionary<string, Guid[]> _runningCommands = new ConcurrentDictionary<string, Guid[]>();

        public Task AddCommandAsync( string callbackId, Guid commandId )
        {
            _runningCommands.AddOrUpdate( callbackId, new Guid[1] { commandId }, ( key, runningCommands ) =>
            {
                return LockFreeAdd( commandId, runningCommands );
            } );
            return Task.FromResult( 0 );
        }

        private static Guid[] LockFreeAdd( Guid valueToAdd, Guid[] currentValues )
        {
            // This is lock-free and should be thread safe.
            int size = currentValues.Length; // ... 6 ]
            Array.Resize( ref currentValues, size + 1 ); // T1: ... 7 ] T2: ... 7 8 ]
            currentValues[size] = valueToAdd; // T1: 7 <- v1 T2: 8 <- v2.
            Array.Sort( currentValues, StringComparer.OrdinalIgnoreCase );
            return currentValues;
        }

        public Task<IReadOnlyCollection<Guid>> GetRunningCommands( string callbackId )
        {
            Guid[] runningCommands;
            _runningCommands.TryGetValue( callbackId, out runningCommands );
            return Task.FromResult<IReadOnlyCollection<Guid>>( runningCommands );
        }

        public Task RemoveCommandAsync( string callbackId, Guid commandId )
        {
            Guid[] runningCommands;
            if( _runningCommands.TryGetValue( callbackId, out runningCommands ) )
            {
                int idx = Array.IndexOf( runningCommands, commandId );
                Array.ConstrainedCopy( runningCommands, idx + 1, runningCommands, idx, runningCommands.Length - 1 );
            }
            return Task.FromResult( 0 );
        }
    }
}
