using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class CommandHandler<T, TResult> : ICommandHandler<T> where T : class
        where TResult : class
    {
        public async Task<object> HandleAsync( ICommandExecutionContext context, T command )
        {
#if DEBUG
            const string msg = "The result of the command must be a nested class of the Command itself named Result.";
            Type resultType = typeof( TResult );
            if( !resultType.IsNested || resultType.Name != "Result" ) throw new InvalidOperationException( msg );
#endif
            var result = await DoHandleAsync( context, command  );
            return result as TResult;
        }

        protected abstract Task<TResult> DoHandleAsync( ICommandExecutionContext context, T command );

        Task<object> ICommandHandler.HandleAsync( ICommandExecutionContext context, object command )
        {
            return HandleAsync( context, (T)command );
        }
    }

    public abstract class CommandHandler<T> : ICommandHandler<T> where T : class
    {
        public async Task<object> HandleAsync( ICommandExecutionContext context, T command )
        {
            await DoHandleAsync( context, command );
            return EmptyResult.Empty;
        }

        protected abstract Task DoHandleAsync( ICommandExecutionContext context, T command );

        Task<object> ICommandHandler.HandleAsync( ICommandExecutionContext context, object command )
        {
            return HandleAsync( context, (T)command );
        }
    }

    internal class EmptyResult
    {
        public static EmptyResult Empty = new EmptyResult();
    }
}
