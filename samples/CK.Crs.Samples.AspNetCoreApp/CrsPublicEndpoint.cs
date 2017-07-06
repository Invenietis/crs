using CK.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route("my-crs-public/[Action]")]
    public class CrsPublicEndpoint<T> : DefaultCrsEndpoint<T> where T : class
    {
        public CrsPublicEndpoint(ICommandDispatcher dispatcher) : base(dispatcher) { }

        [HttpPost, NoAmbientValuesValidation]
        public override Task<CommandResponse> ReceiveCommand([FromBody] T command, string callbackId)
             => base.ReceiveCommand(command, callbackId);
    }
}