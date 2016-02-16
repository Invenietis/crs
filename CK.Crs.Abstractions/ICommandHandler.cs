using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync( object commandContext );
    }

    public interface ICommandHandler<T> : ICommandHandler where T : class
    {
        Task<object> HandleAsync( Command<T> commandContext );
    }

}
