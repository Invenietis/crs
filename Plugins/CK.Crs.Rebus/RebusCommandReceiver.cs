using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;
using Rebus.Bus;
using CK.Crs.Responses;

namespace CK.Crs.Rebus
{
    public class RebusCommandReceiver : ICommandReceiver, IDisposable
    {
        readonly IBus _bus;

        public RebusCommandReceiver( IBus bus )
        {
            _bus = bus;
        }

        public string Name => "Rebus";

        public bool AcceptCommand( ICommandContext context )
        {
            return context.Model.HasRebusTag();
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public async Task<Response> ReceiveCommand( object command, ICommandContext context )
        {
            await _bus.Send( command, context.CreateHeaders() ).ConfigureAwait( false );
            context.Monitor.Trace( "Command sent on the Rebus bus" );
            return new DeferredResponse( context.CommandId, context.CallerId );
        }
    }
}
