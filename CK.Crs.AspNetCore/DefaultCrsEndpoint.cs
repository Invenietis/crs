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

        public virtual async Task<CommandResponse> ReceiveCommand([FromBody] T command, IActivityMonitor monitor, string callerId)
        {
            var context = new CommandContext(Guid.NewGuid(), monitor, callerId);
            await _processor.SendAsync(command, context);

            return new CommandDeferredResponse(context.Id, context.CallerId);
        }
    }
}
