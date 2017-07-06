using CK.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route("my-crs-public/[Action]")]
    public class CrsPublicEndpoint<T> : ICrsEndpoint<T> where T : class
    {
        readonly ICommandDispatcher _processor;

        public CrsPublicEndpoint(ICommandDispatcher processor)
        {
            _processor = processor;
        }

        [HttpPost, NoAmbientValuesValidation] 
        public async Task<CommandResponse> ReceiveCommand( [FromBody] T command, string callbackId )
        {
            var context = new CommandExecutionContext(Guid.NewGuid(), new ActivityMonitor(), callbackId);
            await _processor.SendAsync( command, context );

            return new CommandDeferredResponse( context.Id, context.CallbackId );
        }
    }
}