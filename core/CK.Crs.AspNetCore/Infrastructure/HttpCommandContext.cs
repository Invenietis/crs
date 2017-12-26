using System;
using System.Threading;
using CK.Core;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.AspNetCore
{
    public class HttpCommandContext : CommandContext
    {
        private HttpContext _context;

        public HttpCommandContext( HttpContext context, string commandId, IActivityMonitor activityMonitor, CommandModel model, CallerId callerId  )
            : base( commandId, activityMonitor, model, callerId, context.RequestAborted )
        {
            _context = context;
        }

        public IServiceProvider RequestServices => Context.RequestServices;

        public HttpContext Context => _context;
    }
}
