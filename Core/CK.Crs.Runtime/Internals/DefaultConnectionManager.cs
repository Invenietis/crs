using CK.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class DefaultConnectionManager : ICrsConnectionManager
    {
        readonly ConcurrentBag<CallerId> _activeConnections;

        public DefaultConnectionManager()
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
                return ConnectionAdded?.Invoke( callerId ) ?? Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        public Task RemoveConnection( CallerId callerId )
        {
            if( _activeConnections.TryTake( out callerId ) )
            {
                return ConnectionRemoved?.Invoke( callerId ) ?? Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

    }
}
