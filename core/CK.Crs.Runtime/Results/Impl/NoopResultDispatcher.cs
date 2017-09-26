using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    class NoopResultDispatcher : IResultDispatcher
    {
        public static readonly NoopResultDispatcher Instance = new NoopResultDispatcher();
        public void Send<T>( ICommandContext context, Response<T> response )
        {
            context.Monitor.Trace( "=== NOOP CLIENT DISPATCHER ===" );
            context.Monitor.Trace( response.CommandId.ToString() );
            context.Monitor.Trace( context.CallerId.ToString() );
        }

        public void Broadcast<T>( ICommandContext context, Response<T> response )
        {
            context.Monitor.Trace( "=== NOOP CLIENT DISPATCHER ===" );
            context.Monitor.Trace( response.CommandId.ToString() );
            context.Monitor.Trace( context.CallerId.ToString() );
        }

        public void Dispose()
        {
        }
    }
}
