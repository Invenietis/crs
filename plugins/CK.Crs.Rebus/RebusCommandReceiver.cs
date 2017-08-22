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
            if( context.Model.HasRebusTag() )
            {
                await _bus.Send( command, context.CreateHeaders() );

                return new Response( ResponseType.Asynchronous, context.CommandId );
            }
            return null;
        }
    }
}
