using System.Threading.Tasks;

namespace CK.Crs
{
    class BroadcastResultReceiver : IResultReceiver
    {
        private readonly IClientDispatcher _dispatcher;

        public BroadcastResultReceiver( IClientDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        public string Name => "BroadcastReceiver";

        public Task ReceiveResult( object result, ICommandContext context )
        {
            var response = new Response( ResponseType.Synchronous, context.CommandId )
            {
                Payload = result
            };
            _dispatcher.Broadcast( response );
            return Task.FromResult( response );
        }
    }
}
