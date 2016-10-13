using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CK.Crs.Owin
{
    public class OwinRequestUserNameProvider : IUserNameProvider
    {
        public Task<string> GetAuthenticatedUserNameAsync()
        {
            return Task.FromResult( HttpContext.Current.User.Identity.Name );
        }
    }
}