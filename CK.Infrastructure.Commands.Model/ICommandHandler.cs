using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync( object command );
    }
    
}
