using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public enum AmbientValueCheckResult
    {
        Impersonation,
        Failure,
        Success
    }

    public interface IAmbientValueChecker
    {
        Task<AmbientValueCheckResult> CheckValueAsync( IActivityMonitor monitor, IAmbientValues ambientValues, object model );
    }
}
