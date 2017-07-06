using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.Caching.Memory;

namespace CK.Core
{
    //private void SetInvalidAmbientValuesResponse(IPipeline pipeline, AmbientValueValidationContext context)
    //{
    //    if (context.Rejected)
    //    {
    //        string msg = $"Invalid ambient values detected: {context.RejectReason}";
    //        pipeline.Monitor.Warn().Send(msg);
    //        pipeline.Response.Set(new CommandErrorResponse(msg, pipeline.Action.CommandId));
    //    }
    //    else
    //    {
    //        pipeline.Monitor.Info().Send("Ambient values validator invalidate ambient values, but the last hook from Pipeline.Events.AmbientValuesInvalidated cancel the rejection.");
    //    }
    //}
}
