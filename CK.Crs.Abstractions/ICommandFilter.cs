using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandFilter
    {
        int Order { get; }
        Task OnCommandReceived( CommandContext context );
    }
}