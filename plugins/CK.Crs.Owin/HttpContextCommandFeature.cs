using System;
using Microsoft.Owin;

namespace CK.Crs.Owin
{
    internal class OwinContextCommandFeature : IOwinContextCommandFeature, IRequestServicesCommandFeature
    {
        public OwinContextCommandFeature( IOwinContext owinContext, IServiceProvider applicationServices )
        {
            OwinContext = owinContext;
            RequestServices = applicationServices;
        }

        public IOwinContext OwinContext { get; }

        public IServiceProvider RequestServices { get; }
    }
}
