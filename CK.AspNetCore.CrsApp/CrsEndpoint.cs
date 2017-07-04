using CK.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using Microsoft.AspNetCore.Routing;
using CK.Crs.Samples.AspNetCoreApp.Core;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route("my-crs-slim/[Action]")]
    public class MyCrsSlimController<T> : BrighterCrsController<T> where T : class, ICommand
    {
        public MyCrsSlimController(IAmACommandProcessor processor) : base(processor) { }

        [CrsActionConvention] // Should be the first in order. Will be removed and automatically configured by the Framwework.
        [HttpPost]
        [Authorize]
        [ValidateAmbientValues]
        public override Task<CommandResponse> ReceiveCommand([FromBody] T command, string callbackId)
            => base.ReceiveCommand(command, callbackId);
    }

}
