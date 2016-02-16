using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class CommandHandler<T, TResult> : ICommandHandler<T> where T : class
        where TResult : class
    {
        public async Task<object> HandleAsync( Command<T> commandContext )
        {
#if DEBUG
            const string msg = "The result of the command must be a nested class of the Command itself named Result.";
            Type resultType = typeof( TResult );
            if( !resultType.IsNested || resultType.Name != "Result" ) throw new InvalidOperationException( msg );
#endif
            var result = await DoHandleAsync( commandContext  );
            return result as TResult;
        }

        protected abstract Task<TResult> DoHandleAsync( Command<T> command );

        Task<object> ICommandHandler.HandleAsync( object commandContext )
        {
            return HandleAsync( (Command<T>)commandContext );
        }
    }

    public abstract class CommandHandler<T> : ICommandHandler<T> where T : class
    {
        public async Task<object> HandleAsync( Command<T> commandContext )
        {
            await DoHandleAsync( commandContext );
            return EmptyResult.Empty;
        }

        protected abstract Task DoHandleAsync( Command<T> command );

        Task<object> ICommandHandler.HandleAsync( object commandContext )
        {
            return HandleAsync( (Command<T>)commandContext );
        }
    }

    internal class EmptyResult
    {
        public static EmptyResult Empty = new EmptyResult();

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
