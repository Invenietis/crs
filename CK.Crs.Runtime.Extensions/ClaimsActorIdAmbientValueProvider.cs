using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CK.Crs
{
    public class ClaimsActorIdAmbientValueProvider : ActorIdAmbientValueProvider
    {
        readonly string  _userIdClaimType;
        readonly Func<ClaimsPrincipal> _claimsPrincipalAccessor;

        protected string UserIdClaimType => _userIdClaimType;

        public ClaimsActorIdAmbientValueProvider( IActorIdProvider actorIdProvider )
            : this( actorIdProvider, () => ClaimsPrincipal.Current, ClaimTypes.NameIdentifier )
        {

        }
        public ClaimsActorIdAmbientValueProvider( IActorIdProvider actorIdProvider, Func<ClaimsPrincipal> claimsPrincipalAccessor )
            : this( actorIdProvider, claimsPrincipalAccessor, ClaimTypes.NameIdentifier )
        {
        }

        public ClaimsActorIdAmbientValueProvider( IActorIdProvider actorIdProvider, Func<ClaimsPrincipal> claimsPrincipalAccessor, string userIdClaimType ) :
            this( actorIdProvider, claimsPrincipalAccessor, new UserNameProvider( claimsPrincipalAccessor ), userIdClaimType )
        {
        }

        public ClaimsActorIdAmbientValueProvider( IActorIdProvider actorIdProvider, Func<ClaimsPrincipal> claimsPrincipalAccessor, IUserNameProvider userNameProvider, string userIdClaimType ) : base( actorIdProvider, userNameProvider )
        {
            _claimsPrincipalAccessor = claimsPrincipalAccessor;
            _userIdClaimType = userIdClaimType;
        }

        protected override Task<UserIdResult> PreReadUserIdAsync( string userName, IAmbientValues values )
        {
            var claimsPrincipal = _claimsPrincipalAccessor();
            if( claimsPrincipal != null )
            {
                if( claimsPrincipal.HasClaim( p => p.Type == UserIdClaimType ) )
                {
                    int actorId = 0;
                    return Task.FromResult( new UserIdResult
                    {
                        IsValid = int.TryParse( claimsPrincipal.FindFirst( UserIdClaimType ).Value, out actorId ) && actorId != 0,
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
