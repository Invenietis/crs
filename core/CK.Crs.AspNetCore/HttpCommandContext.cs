using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CK.Core;
using Microsoft.AspNetCore.Http;

namespace CK.Crs
{
    sealed class HttpCommandContext : CommandContext, IHttpCommandContext
    {
        readonly HttpContext _context;
        public HttpCommandContext(
            HttpContext context,
            string commandId,
            IActivityMonitor activityMonitor,
            CommandModel model,
            CallerId correlation,
            CancellationToken token = default( CancellationToken ) ) : base( commandId, activityMonitor, model, correlation, token )
        {
            _context = context;
        }

        /// <summary>
        /// Gets the <see cref="HttpContext"/> of the current incoming command
        /// </summary>
        /// <returns></returns>
        public HttpContext GetHttpContext() => _context;
    }
}
