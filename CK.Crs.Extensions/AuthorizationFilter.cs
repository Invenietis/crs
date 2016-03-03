using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CK.Authentication;
using CK.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CK.Crs.Extensions
{
    public class AuthorizationFilter : ICommandFilter
    {
        readonly IAuthorizationService _authorizationService;
        readonly IPrincipalAccessor _principalAccessor;

        public AuthorizationFilter( IAuthorizationService authorizationService, IPrincipalAccessor principalAccessor )
        {
            _authorizationService = authorizationService;
            _principalAccessor = principalAccessor;
        }

        public async Task OnCommandReceived( ICommandFilterContext context )
        {
            var user = _principalAccessor.User as ClaimsPrincipal;
            if( user == null ) throw new NotSupportedException( "Only claim based authentication is supported" );

            IReadOnlyCollection<ProtectedResource> resources = GetSecuredResourceDescription( context ).ToArray();
            IReadOnlyCollection<IAuthorizationRequirement> requirements = ReadRequirements( context );

            if( resources.Count > 0 )
            {
                foreach( var resource in resources )
                {
                    var resourceAuthorized = await _authorizationService.AuthorizeAsync( user, resource, requirements );
                    if( !resourceAuthorized )
                    {
                        context.Reject( $"User not authorized for resource {resource.ResourceType}." );
                        return;
                    }
                }
            }

            var authorized = await _authorizationService.AuthorizeAsync( user, null, requirements );
            if( !authorized )
            {
                context.Reject( "User not authorized." );
                return;
            }
        }

        private IEnumerable<ProtectedResource> GetSecuredResourceDescription( ICommandFilterContext context )
        {
            var securityAttributes = context.Description.Descriptor.CommandType
                    .GetProperties()
                    .Select(
                        m => new
                        {
                            Property = m,
                            Attributes = m
                                .GetCustomAttributes( typeof( SecuredResourceAttribute ), true )
                                .OfType<SecuredResourceAttribute>()
                        } );

            foreach( var securityAttribute in securityAttributes )
            {
                var filter = securityAttribute.Attributes.FirstOrDefault();
                if( filter != null )
                {
                    object value = securityAttribute.Property.GetGetMethod().Invoke( context.Command, null );
                    yield return new ProtectedResource
                    {
                        ResourceId = value,
                        ResourceType = filter.ResourceType
                    };
                }
            }
        }

        private IReadOnlyCollection<IAuthorizationRequirement> ReadRequirements( ICommandFilterContext context )
        {
            return context.Description.Descriptor.Permissions.OfType<IAuthorizationRequirement>().ToArray();
        }

        public int Order
        {
            get { return -1; }
        }



    }
}
