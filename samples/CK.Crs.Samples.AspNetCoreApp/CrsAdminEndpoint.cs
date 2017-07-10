using CK.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route( "my-crs-admin/[Action]" )]
    public class CrsAdminEndpoint<T> : DefaultCrsEndpoint<T> where T : class
    {
        public CrsAdminEndpoint( IBus dispatcher ) : base( dispatcher ) { }

        [HttpPost, Authorize]
        public override Task<Response> ReceiveCommand( [FromBody] T command, IActivityMonitor monitor, string callerId )
            => base.ReceiveCommand( command, monitor, callerId );
    }

}
