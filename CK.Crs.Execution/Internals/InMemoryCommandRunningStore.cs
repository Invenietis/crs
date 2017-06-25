using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
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
            _runningCommands.TryGetValue(callbackId, out List<Guid> runningCommands);
            return Task.FromResult<IReadOnlyCollection<Guid>>( runningCommands );
        }

        public Task RemoveCommandAsync( string callbackId, Guid commandId )
        {
            if (_runningCommands.TryGetValue(callbackId, out List<Guid> runningCommands))
            {
                runningCommands.Remove(commandId);
            }
            return Task.FromResult( 0 );
        }
    }
}
