using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class UserNameProvider : IUserNameProvider
    {
        public Func<ClaimsPrincipal> ClaimsPrincipalAccessor { get; }

        public UserNameProvider( Func<ClaimsPrincipal> claimsPrincipalAccessor )
        {
            ClaimsPrincipalAccessor = claimsPrincipalAccessor;
        }

        public Task<string> GetAuthenticatedUserNameAsync()
        {
            var ctx = ClaimsPrincipalAccessor();
            return Task.FromResult( ctx.Identity.Name );
        }

    }
}