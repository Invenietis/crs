using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using CK.Authentication;

namespace CK.Crs.Tests
{
    internal class TestPrincipalAccessor : IPrincipalAccessor
    {
        public IPrincipal User
        {
            get
            {
                return ClaimsPrincipal.Current;
            }
        }
    }
}