using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;

namespace CK.Crs.Owin
{
    public class ClaimsBasedActorIdProvider : ActorIdProvider
    {
        Func<IOwinContext> _owinContextProvider;
        public ClaimsBasedActorIdProvider( IActorIdProvider actorIdProvider, Func<IOwinContext> owinContextProvider ) : 
            this( actorIdProvider, owinContextProvider, new UserNameProvider( owinContextProvider ) )
        {
        }

        public ClaimsBasedActorIdProvider( IActorIdProvider actorIdProvider, Func<IOwinContext> owinContextProvider, IUserNameProvider userNameProvider ) : base( actorIdProvider, userNameProvider )
        {
            _owinContextProvider = owinContextProvider;
        }

        protected override Task<UserIdResult> PreReadUserIdAsync( string userName, IAmbientValues values )
        {
            var ctx = _owinContextProvider();
            var claimsPrincipal = ctx.Authentication.User as ClaimsPrincipal;
            if( claimsPrincipal != null )
            {
                if( claimsPrincipal.HasClaim( p => p.Type == ClaimTypes.NameIdentifier ) )
                {
                    int actorId = 0;
                    return Task.FromResult( new UserIdResult
                    {
                        IsValid = int.TryParse( claimsPrincipal.FindFirst( ClaimTypes.NameIdentifier ).Value, out actorId ) && actorId != 0,
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
