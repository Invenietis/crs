using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace CK.Crs.Owin
{
    public class UserNameProvider : IUserNameProvider
    {
        public UserNameProvider( Func<IOwinContext> owinContextProvider )
        {
            Context = owinContextProvider;
        }

        public Task<string> GetAuthenticatedUserNameAsync()
        {
            var ctx = Context();
            return Task.FromResult( ctx.Authentication.User.Identity.Name );
        }

        public Func<IOwinContext> Context { get; set; }
    }
}