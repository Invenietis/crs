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
        public HttpCommandContext( HttpContext context, Guid guid, IActivityMonitor activityMonitor, CommandModel model, string callerId, CancellationToken token = default( CancellationToken ) ) : base( guid, activityMonitor, model, callerId, token )
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
