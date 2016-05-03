using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CK.Crs.Owin
{
    public class ClaimsBasedActorIdProvider : ActorIdProvider
    {
        public ClaimsBasedActorIdProvider( IActorIdProvider actorIdProvider ) : this( actorIdProvider, new OwinRequestUserNameProvider() )
        {
        }

        public ClaimsBasedActorIdProvider( IActorIdProvider actorIdProvider, IUserNameProvider userNameProvider ) : base( actorIdProvider, userNameProvider )
        {
        }

        protected override Task<UserIdResult> PreReadUserIdAsync( string userName, IAmbientValues values )
        {
            var claimsPrincipal = HttpContext.Current.User as ClaimsPrincipal;
            if( claimsPrincipal != null )
            {
                if( claimsPrincipal.HasClaim( p => p.Type == "CK.ActorId" ) )
                {
                    int actorId = 0;
                    return Task.FromResult( new UserIdResult
                    {
                        IsValid = int.TryParse( claimsPrincipal.FindFirst( "CK.ActorId" ).Value, out actorId ) && actorId != 0,
                        UserId = actorId
                    } );
                }

                return Task.FromResult( new UserIdResult
                {
                    IsValid = false,
                    UserId = 0
                } );
            }
            return base.PreReadUserIdAsync( userName, values );
        }

        protected override Task<UserIdResult> PostReadUserIdAsync( int userId, string userName, IAmbientValues values )
        {
            return base.PostReadUserIdAsync( userId, userName, values );
        }
    }
}
