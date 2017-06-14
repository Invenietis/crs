
using CK.Crs;
using CK.Crs.Runtime;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsConfigurationExtensions
    {
        public static void UseJsonNet(this CommandReceiverOption option)
        {
            option.Services.Add(ServiceDescriptor.Singleton<IJsonConverter, JsonNetConverter>());
        }

    }
}
