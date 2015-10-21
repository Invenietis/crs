using System.Threading.Tasks;

namespace CK.Infrastructure.Commands.Framework
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync( object command );
    }
    
}
