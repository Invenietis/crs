using System.Security.Principal;
using Microsoft.AspNet.Http;

namespace CK.Crs
{
    public class AspNetPrincipalAccessor : IPrincipalAccessor
    {
        readonly IHttpContextAccessor _accessor;

        public AspNetPrincipalAccessor( IHttpContextAccessor accessor )
        {
            _accessor = accessor;
        }

        public IPrincipal User
        {
            get
            {
                return _accessor.HttpContext.User;
            }
        }
    }
}
