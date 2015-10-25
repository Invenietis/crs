using System;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class EmptyResult
    {
        public static EmptyResult Empty = new EmptyResult();

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public abstract class CommandHandler<T, TResult> : ICommandHandler
        where T : class
        where TResult : class
    {
        public async Task<object> HandleAsync( object command )
        {
#if DEBUG
            const string msg = "The result of the command must be a nested class of the Command itself named Result.";
            Type resultType = typeof( TResult );
            if( !resultType.IsNested || resultType.Name != "Result" ) throw new InvalidOperationException( msg );
#endif
            var result = await DoHandleAsync( command as T );
            return result as TResult;
        }

        protected abstract Task<TResult> DoHandleAsync( T command );
    }

    public abstract class CommandHandler<T> : ICommandHandler where T : class
    {
        public async Task<object> HandleAsync( object command )
        {
            await DoHandleAsync( command as T );
            return EmptyResult.Empty;
        }

        protected abstract Task DoHandleAsync( T command );
    }

}
