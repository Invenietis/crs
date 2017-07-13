using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public interface ICrsCoreBuilder
    {
        IServiceCollection Services { get; }
        IRequestRegistry Registry { get; }
        ICrsModel Model { get; }
    }
}
