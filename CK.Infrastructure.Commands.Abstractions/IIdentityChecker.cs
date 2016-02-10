using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public class IdentityCheckResult
    {
        public bool Success { get; set; }
    }

    public interface IIdentityChecker
    {
        Task<IdentityCheckResult> CheckIdentityAsync( IActivityMonitor monitor, ClaimsPrincipal principal, object model );
    }
}
