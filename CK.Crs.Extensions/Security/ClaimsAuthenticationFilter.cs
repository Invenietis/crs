using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Authentication;

namespace CK.Crs
{
    public class ClaimsAuthenticationFilter : ICommandFilter
    {
        readonly IIdentityChecker _identityChecker;
        readonly IPrincipalAccessor _principalAccessor;

        public ClaimsAuthenticationFilter( IPrincipalAccessor principalAccessor, IIdentityChecker identityChecker )
        {
            _principalAccessor = principalAccessor;
            _identityChecker = identityChecker;
        }

        public int Order
        {
            get { return 0; }
        }

        public async Task OnCommandReceived( CommandContext context )
        {
            var monitor = context.Command.Monitor;
            var model = context.Command.Model;

            var principal = _principalAccessor.User;
            if( principal == null )
                throw new InvalidOperationException( "An IPrincipal must be provided..." );

            var claimPrincipal = principal as ClaimsPrincipal;
            if( claimPrincipal == null )
                throw new NotSupportedException( "Only claims-based authentication is supported" );

            var result = await _identityChecker.CheckIdentityAsync( monitor, claimPrincipal, model );

            if( !result.Success )
            {
                string errorMessage = $"Identity check failed...";
                context.SetResult( new ValidationResult( errorMessage ) );
            }
        }
    }
}
