using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route( "crs-dispatcher" )]
    public class CrsDispatcherEndpoint<T> : IHttpCommandReceiver<T> where T : class
    {
        ICommandReceiver _receiver;
        public CrsDispatcherEndpoint( ICommandReceiver receiver )
        {
            _receiver = receiver;
        }

        [HttpPost( "[Action]" ), NoAmbientValuesValidation]
        public Task<Response> ReceiveCommand( T command, ICommandContext context  )
        {
            return _receiver.ReceiveCommand( command, context );
        }
    }
}
