using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CK.Authentication;
using CK.Authorization;
using Microsoft.AspNetCore.Authorization;
using CK.Core;

namespace CK.Crs
{
    public class ProtectedResourceAuthorizationFilter : ICommandFilter
    {
        readonly IAuthorizationService _authorizationService;

        public ProtectedResourceAuthorizationFilter( IAuthorizationService authorizationService )
        {
            _authorizationService = authorizationService;
        }

        public async Task OnCommandReceived( ICommandFilterContext context )
        {
            if( context.User == null ) throw new NotSupportedException( "A command MUST be linked to a principal, even if anonymous command is issued." );

            IReadOnlyCollection<ProtectedResource> resources = GetSecuredResourceDescription( context ).ToArray();
            IReadOnlyCollection<IAuthorizationRequirement> requirements = ReadRequirements( context );
            context.Monitor.Trace().Send( "{0} requirements found for this command.", requirements.Count );
            context.Monitor.Trace().Send( "{0} protected resource(s) found for this command.", resources.Count );
            if( resources.Count > 0 )
            {
                foreach( var resource in resources )
                {
                    using( context.Monitor.OpenTrace().Send( "Reading authorization for resource {0}={1}...", resource.ResourceType, resource.ResourceId ) )
                    {
                        var resourceAuthorized = await _authorizationService.AuthorizeAsync( context.User, resource, requirements );
                        if( !resourceAuthorized )
                        {
                            string msg = String.Format("User not authorized for resource {0}={1}...", resource.ResourceType, resource.ResourceId );
                            context.Reject( msg );
                            return;
                        }
                    }
                }
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
            return context.Description.Descriptor.GetAuthorizationRequirement();
        }

        public int Order
        {
            get { return -1; }
        }



    }
}
