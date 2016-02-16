using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CK.Authentication;
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
            var monitor = executionContext.RuntimeContext.Monitor;
            var model = executionContext.RuntimeContext.Command;
            var principal = _accessor.HttpContext.User;
            var result = await _identityChecker.CheckIdentityAsync(  monitor, principal, model );

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
