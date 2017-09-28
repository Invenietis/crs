using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class SendResultReceiver : IResultReceiver
    {
        private readonly IResultDispatcherSelector _dispatcherSelector;

        public SendResultReceiver( IResultDispatcherSelector dispatcherSelector )
        {
            _dispatcherSelector = dispatcherSelector;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name => "SendResultReceiver";

        public Task ReceiveResult<T>( T result, ICommandContext context )
        {
            var response = new Response<T>( context.CommandId )
            {
                Payload = result
            };
            _dispatcherSelector.SelectDispatcher( context ).Send( context, response );
            return Task.FromResult( response );
        }
    }
}
