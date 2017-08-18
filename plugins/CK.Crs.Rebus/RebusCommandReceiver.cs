using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;
using R = Rebus;
using Rebus.Config;
using Rebus.Activation;

namespace CK.Crs
{
    public class RebusCommandReceiver : ICommandReceiver, IDisposable
    {
        readonly R.Bus.IBus _bus;

        public RebusCommandReceiver( RebusConfigurer configurer, IContainerAdapter adapter )
        {
            _bus = configurer.Start();
            adapter.SetBus( _bus );
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public async Task<Response> ReceiveCommand<T>( T command, ICommandContext context ) where T : class
        {
            if( context.Model.HasRebusQueueTag() )
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
                //{ R.Messages.Headers.ReturnAddress, "" },
                { R.Messages.Headers.MessageId, context.CommandId.ToString() }
            };
        }
    }
}
