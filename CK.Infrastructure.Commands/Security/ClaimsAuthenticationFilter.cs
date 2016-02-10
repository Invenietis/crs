using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace CK.Infrastructure.Commands
{
    public class ClaimsAuthenticationFilter : ICommandFilter
    {
        readonly IIdentityChecker _identityChecker;
        readonly IHttpContextAccessor _accessor;

        public ClaimsAuthenticationFilter( IHttpContextAccessor accessor, IIdentityChecker identityChecker )
        {
            _accessor = accessor;
            _identityChecker = identityChecker;
        }

        public int Order
        {
            get { return 0; }
        }

        public async Task OnCommandReceived( CommandExecutionContext executionContext )
        {
            var result = await _identityChecker.CheckIdentityAsync( 
                executionContext.RuntimeContext.Monitor,
                _accessor.HttpContext.User,
                executionContext.RuntimeContext.Command );

            if( !result.Success )
            {
                string errorMessage = $"Identity check failed...";
                executionContext.SetResponse(
                    new CommandInvalidResponse( executionContext.RuntimeContext,
                    new ValidationResult( errorMessage ) ) );
            }
        }
    }
}
