using CK.Crs.Responses;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Results
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

        public Task ReceiveError( Exception ex, ICommandContext context )
        {
            var response = new ErrorResponse( ex, context.CommandId );
            return _dispatcherSelector.SelectDispatcher( context ).Send( context, response );
        }

        public Task ReceiveResult<T>( T result, ICommandContext context )
        {
            var response = new Response<T>( context.CommandId, result );
            return _dispatcherSelector.SelectDispatcher( context ).Send( context, response );
        }
    }
}
