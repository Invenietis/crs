using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IEventHandler<T>: IEventHandler
    {
        Task HandleAsync( T request, ICommandContext context );
    }
}
