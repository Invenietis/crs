using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    public interface ICommandRunningStore
    {
        Task AddCommandAsync( string callbackId, Guid commandId );

        Task RemoveCommandAsync( string callbackId, Guid commandId );

        Task<IReadOnlyCollection<Guid>> GetRunningCommands( string callbackId );
    }

    class InMemoryCommandRunningStore : ICommandRunningStore
    {
        public static ConcurrentDictionary<string, List<Guid>> _runningCommands = new ConcurrentDictionary<string, List<Guid>>();

        public Task AddCommandAsync( string callbackId, Guid commandId )
        {
            _runningCommands.AddOrUpdate( callbackId, new List<Guid> { commandId }, ( key, runningCommands ) =>
            {
                return LockFreeAdd( commandId, runningCommands );
            } );
            return Task.FromResult( 0 );
        }

        private static List<Guid> LockFreeAdd( Guid valueToAdd, List<Guid> currentValues )
        {
            currentValues.Add( valueToAdd );
            return currentValues;
        }

        public Task<IReadOnlyCollection<Guid>> GetRunningCommands( string callbackId )
        {
            List<Guid> runningCommands;
            _runningCommands.TryGetValue( callbackId, out runningCommands );
            return Task.FromResult<IReadOnlyCollection<Guid>>( runningCommands );
        }

        public Task RemoveCommandAsync( string callbackId, Guid commandId )
        {
            List<Guid> runningCommands;
            if( _runningCommands.TryGetValue( callbackId, out runningCommands ) )
            {
                runningCommands.Remove( commandId );
            }
            return Task.FromResult( 0 );
        }
    }
}
