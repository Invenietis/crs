using CK.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class DefaultCrsEndpoint<T> : ICrsEndpoint<T> where T : class
    {
        readonly ICommandDispatcher _processor;

        public DefaultCrsEndpoint(ICommandDispatcher processor)
        {
            _processor = processor;
        }

        public virtual async Task<CommandResponse> ReceiveCommand([FromBody] T command, string callbackId)
        {
            var context = new CommandExecutionContext(Guid.NewGuid(), new ActivityMonitor(), callbackId);
            await _processor.SendAsync(command, context);

            return new CommandDeferredResponse(context.Id, context.CallbackId);
        }
    }
}
