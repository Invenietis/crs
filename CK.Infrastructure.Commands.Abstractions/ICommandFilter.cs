using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandFilter
    {
        int Order { get; }
        Task OnCommandReceived( CommandExecutionContext executionContext );
    }
}