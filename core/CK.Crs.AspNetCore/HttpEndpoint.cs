using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class HttpEndpoint<T> : IHttpCommandReceiver<T>
    {
        [HttpPost( "[Action]" )]
        public virtual Task<Response> ReceiveCommand( [FromBody] T command, HttpCommandContext context )
        {
            var receiver = context.RequestServices.GetRequiredService<ICommandReceiver>();
            return receiver.ReceiveCommand( command, context );
        }

        Task<Response> IHttpCommandReceiver<T>.ReceiveCommand( [FromBody] T command, ICommandContext context )
        {
            return ReceiveCommand( command, (HttpCommandContext)context );
        }
    }
}
