using CK.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route("my-crs-public/[Action]")]
    public class CrsPublicEndpoint<T> : BrighterCrsController<T> where T : class, ICommand
    {
        public CrsPublicEndpoint(IAmACommandProcessor processor) : base(processor) { }

        [HttpPost]
        public override Task<CommandResponse> ReceiveCommand([FromBody] T command, string callbackId)
            => base.ReceiveCommand(command, callbackId);
    }

}
