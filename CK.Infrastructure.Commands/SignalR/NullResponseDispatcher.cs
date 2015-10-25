using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class NullResponseDispatcher : ICommandResponseDispatcher
    {
        public Task DispatchAsync( string callbackId, ICommandResponse response, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            return Task.FromResult( 0 );
        }
    }
}
