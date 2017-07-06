using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public interface ICrsCoreBuilder
    {
        IServiceCollection Services { get; }
        ICommandRegistry Registry { get; }
    }
}
