using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class NoopResultDispatcher : IResultDispatcher
    {
        public static readonly NoopResultDispatcher Instance = new NoopResultDispatcher();
        public Task Send<T>( ICommandContext context, Response<T> response )
        {
            context.Monitor.Trace( "=== NOOP CLIENT DISPATCHER ===" );
            context.Monitor.Trace( response.CommandId.ToString() );
            context.Monitor.Trace( context.CallerId.ToString() );

            return Task.CompletedTask;
        }

        public Task Broadcast<T>( ICommandContext context, Response<T> response )
        {
            context.Monitor.Trace( "=== NOOP CLIENT DISPATCHER ===" );
            context.Monitor.Trace( response.CommandId.ToString() );
            context.Monitor.Trace( context.CallerId.ToString() );

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
