using CK.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class ConnectionManager : ICrsConnectionManager
    {
        ConcurrentBag<CallerId> _activeConnections;

        public ConnectionManager()
        {
            _activeConnections = new ConcurrentBag<CallerId>();
        }

        public event Func<CallerId, Task> ConnectionAdded;

        public event Func<CallerId, Task> ConnectionRemoved;

        public Task AddConnection( CallerId callerId )
        {
            if( !_activeConnections.TryPeek( out callerId ) )
            {
                _activeConnections.Add( callerId );
                return ConnectionAdded?.Invoke( callerId );
            }
            return Task.CompletedTask;
        }

        public Task RemoveConnection( CallerId callerId )
        {
            if( _activeConnections.TryTake( out callerId ) )
            {
                return ConnectionRemoved?.Invoke( callerId );
            }
            return Task.CompletedTask;
        }

    }
}
