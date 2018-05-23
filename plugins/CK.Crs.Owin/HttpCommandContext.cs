using System;
using System.Threading;
using CK.Core;
using Microsoft.Owin;

namespace CK.Crs.Owin
{
    class OwinCommandContext : CommandContext
    {

        public OwinCommandContext( IOwinContext context, string commandId, IActivityMonitor activityMonitor, ICommandModel model, CallerId callerId )
            : base( commandId, activityMonitor, model, callerId, CancellationToken.None )
        {
        }
    }
}
