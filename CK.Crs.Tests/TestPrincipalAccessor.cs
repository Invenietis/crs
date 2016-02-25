using System;
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
                return Thread.CurrentPrincipal;
            }
        }
    }
}