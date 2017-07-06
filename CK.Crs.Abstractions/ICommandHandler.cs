using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync( ICommandContext commandContext, object command );
    }

    public interface ICommandHandler<T>
    {
        Task<object> HandleAsync( ICommandContext commandContext, T command );
    }

}
