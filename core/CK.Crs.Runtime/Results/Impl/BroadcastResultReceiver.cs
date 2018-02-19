using CK.Crs.Responses;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Results
{
    class BroadcastResultReceiver : IResultReceiver
    {
        private readonly IResultDispatcherSelector _dispatcher;

        public BroadcastResultReceiver( IResultDispatcherSelector dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public string Name => "BroadcastReceiver";

        public Task ReceiveError( Exception ex, ICommandContext context )
        {
            var response = new ErrorResponse( ex, context.CommandId );
            return _dispatcher.SelectDispatcher( context ).Broadcast( context, response );
        }

        public Task ReceiveResult<T>( T result, ICommandContext context )
        {
            var response = new Response<T>( context.CommandId, result );
            return _dispatcher.SelectDispatcher( context ).Broadcast( context, response );
        }
    }
}
