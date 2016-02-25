using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync( ICommandExecutionContext commandContext, object command );
    }

    public interface ICommandHandler<T> : ICommandHandler where T : class
    {
        Task<object> HandleAsync( ICommandExecutionContext commandContext, T command );
    }

}
