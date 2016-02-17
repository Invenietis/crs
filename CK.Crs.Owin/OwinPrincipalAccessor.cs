using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace CK.Crs
{
    public class OwinPrincipalAccessor : IPrincipalAccessor
    {
        readonly Func<IOwinContext> _owinContext;
        public OwinPrincipalAccessor( Func<IOwinContext> owinContext )
        {
            _owinContext = owinContext;
        }

        public IOwinContext OwinContext
        {
            get { return _owinContext(); }
        }

        public IPrincipal User
        {
            get { return OwinContext.Authentication.User; }
        }
    }
}
