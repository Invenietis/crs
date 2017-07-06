using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using System;

namespace CK.Crs
{
    public class BrighterCommandFactory : IAmAHandlerFactoryAsync
    {
        readonly IServiceProvider _services;

        public BrighterCommandFactory( IServiceProvider services )
        {
            _services = services;
        }

        public IHandleRequestsAsync Create(Type handlerType)
        {
            return ActivatorUtilities.CreateInstance(_services, handlerType) as IHandleRequestsAsync;
        }

        public void Release(IHandleRequestsAsync handler)
        {
            var d = handler as IDisposable;
            d?.Dispose();
        }
    }
}
