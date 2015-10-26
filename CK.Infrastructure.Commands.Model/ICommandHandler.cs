using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandHandler
    {
        Task<object> HandleAsync( object commandContext );
    }

    public interface ICommandHandler<T> : ICommandHandler where T : class
    {
        Task<object> HandleAsync( CommandContext<T> commandContext );
    }

}
