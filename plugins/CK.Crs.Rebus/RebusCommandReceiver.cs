using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;
using R = Rebus;
using Rebus.Config;
using Rebus.Activation;
using Rebus.Bus;

namespace CK.Crs
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
            await _bus.Send( command, context.CreateHeaders() );
            context.Monitor.Trace( "Command sent on the Rebus bus" );
            return new DeferredResponse( context.CommandId, context.CallerId );
        }
    }
}
