using System;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public class HttpCommandContext : CommandContext
    {
        private IServiceProvider _requestServices;

        public HttpCommandContext( IServiceProvider requestServices, string commandId, IActivityMonitor activityMonitor, CommandModel model, CallerId callerId, CancellationToken token = default( CancellationToken ) ) : base( commandId, activityMonitor, model, callerId, token )
        {
            _requestServices = requestServices;
        }

        public IServiceProvider RequestServices => _requestServices;
    }
}
