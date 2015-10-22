using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public abstract class CommandHandler<T, TResult> : ICommandHandler
        where T : class
        where TResult : class
    {
        public async Task<object> HandleAsync( object command )
        {
            var result = await DoHandleAsync( command as T );
            return result as TResult;
        }

        public abstract Task<TResult> DoHandleAsync( T command );
    }
}
