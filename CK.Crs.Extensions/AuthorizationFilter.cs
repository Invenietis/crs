using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CK.Authentication;
using CK.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CK.Crs.Extensions
{
    public class AuthorizationFilter : ICommandFilter
    {
        readonly IAuthorizationService _authorizationService;

        public AuthorizationFilter( IAuthorizationService authorizationService )
        {
            _authorizationService = authorizationService;
        }

        public async Task OnCommandReceived( ICommandFilterContext context )
        {
            if( context.User == null ) throw new NotSupportedException( "A command MUST be linked to a principal, even if anonymous command is issued." );

            IReadOnlyCollection<ProtectedResource> resources = GetSecuredResourceDescription( context ).ToArray();
            IReadOnlyCollection<IAuthorizationRequirement> requirements = ReadRequirements( context );

            if( resources.Count > 0 )
            {
                foreach( var resource in resources )
                {
                    var resourceAuthorized = await _authorizationService.AuthorizeAsync( context.User, resource, requirements );
                    if( !resourceAuthorized )
                    {
                        context.Reject( $"User not authorized for resource {resource.ResourceType}." );
                        return;
                    }
                }
            }

            var authorized = await _authorizationService.AuthorizeAsync( context.User, null, requirements );
            if( !authorized )
            {
                context.Reject( "User not authorized." );
                return;
            }
        }

        private IEnumerable<ProtectedResource> GetSecuredResourceDescription( ICommandFilterContext context )
        {
            var securityAttributes = context.Description.Descriptor.CommandType.GetTypeInfo()
                    .DeclaredProperties
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
            return context.Description.Descriptor.ExtraData.OfType<IAuthorizationRequirement>().ToArray();
        }

        public int Order
        {
            get { return -1; }
        }



    }
}
