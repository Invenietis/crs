using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class DefaultResultReceiver : IResultReceiver
    {
        private readonly IClientDispatcher _dispatcher;

        public DefaultResultReceiver( IClientDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name => "Peer2PeerResultDispatcher";

        public Task ReceiveResult<T>( T result, ICommandContext context )
        {
            var response = new Response<T>( context.CommandId )
            {
                Payload = result
            };
            _dispatcher.Send( context.CallerId, response );
            return Task.FromResult( response );
        }
    }
}
