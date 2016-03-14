using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Authentication;
using Microsoft.AspNetCore.Http;

namespace CK.Crs
{
    public class ActorIdProvider : IAmbientValueProvider
    {
        readonly IAuthenticationStore _authenticationStore;
        readonly IHttpContextAccessor _httpContextAccessor;

        public ActorIdProvider( IAuthenticationStore authenticationStore, IHttpContextAccessor httpContextAccessor )
        {
            _authenticationStore = authenticationStore;
            _httpContextAccessor = httpContextAccessor;
        }

        public const int Default = 0;

        public async Task<object> GetValueAsync( IAmbientValues values )
        {
            var currentIdentity = _httpContextAccessor.HttpContext.User.Identity;

            var user = await _authenticationStore.GetUserByName( currentIdentity.Name );
            return user?.UserId;
        }
    }
}
