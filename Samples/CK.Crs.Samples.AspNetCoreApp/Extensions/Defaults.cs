using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Claims;

namespace CK.Crs.Samples.AspNetCoreApp.Extensions
{
    public class Defaults : IActorIdProvider, IUserNameProvider
    {
        public Task<string> GetAuthenticatedUserNameAsync()
        {

            return Task.FromResult( ClaimsPrincipal.Current != null ? ClaimsPrincipal.Current.Identity.Name : String.Empty );
        }

        public Task<int> GetUserIdAsync( string userName, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            return Task.FromResult<int>( 0 );
        }
    }
}
