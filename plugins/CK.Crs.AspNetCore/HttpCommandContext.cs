using System;
using System.Threading;
using CK.Core;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.AspNetCore
{
    class HttpCommandContext : CommandContext
    {

        public HttpCommandContext( HttpContext context, string commandId, IActivityMonitor activityMonitor, ICommandModel model, IEndpointModel endpointModel, CallerId callerId  )
            : base( commandId, activityMonitor, model, endpointModel, callerId, context.RequestAborted )
        {
        }
    }
}
