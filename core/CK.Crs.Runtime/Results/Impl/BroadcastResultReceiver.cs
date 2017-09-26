using System.Threading.Tasks;

namespace CK.Crs
{
    class BroadcastResultReceiver : IResultReceiver
    {
        private readonly IResultDispatcherSelector _dispatcher;

        public BroadcastResultReceiver( IResultDispatcherSelector dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public string Name => "BroadcastReceiver";

        public Task ReceiveResult<T>( T result, ICommandContext context )
        {
            var response = new Response<T>( context.CommandId )
            {
                Payload = result
            };
            _dispatcher.SelectDispatcher( context ).Broadcast( context, response );
            return Task.FromResult( response );
        }
    }
}
