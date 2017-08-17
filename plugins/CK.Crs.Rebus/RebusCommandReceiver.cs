using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;
using R = Rebus;

namespace CK.Crs
{
    public class RebusCommandReceiver : ICommandReceiver
    {
        readonly R.Bus.IBus _bus;

        public RebusCommandReceiver( R.Bus.IBus bus )
        {
            _bus = bus;
        }

        public async Task<Response> ReceiveCommand<T>( T command, ICommandContext context ) where T : class
        {
            if( context.Model.HasAsyncTag() )
            {
                var h = ToHeaders( context );
                await _bus.Send( command, h );

                return new Response( ResponseType.Asynchronous, context.CommandId );
            }
            return null;
        }

        private Dictionary<string, string> ToHeaders( ICommandContext context )
        {
            return new Dictionary<string, string>
            {
                // CRS Headers
                { nameof( ICommandContext.CallerId ), context.CallerId },
                { nameof( ICommandContext.CommandId ), context.CommandId.ToString() },
                { nameof( ICommandContext.Monitor ), context.Monitor.DependentActivity().CreateToken().ToString() },
                // Rebus Headers
                { R.Messages.Headers.ReturnAddress, "" },
                { R.Messages.Headers.MessageId, context.CommandId.ToString() }
            };
        }
    }
}
