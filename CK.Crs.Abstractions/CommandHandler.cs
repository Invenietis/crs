using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class CommandHandler<T, TResult> : ICommandHandler<T> where T : class
        where TResult : class
    {
        public async Task<object> HandleAsync( ICommandExecutionContext commandContext, T command )
        {
#if DEBUG
            const string msg = "The result of the command must be a nested class of the Command itself named Result.";
            Type resultType = typeof( TResult );
            if( !resultType.IsNested || resultType.Name != "Result" ) throw new InvalidOperationException( msg );
#endif
            var result = await DoHandleAsync( commandContext, command  );
            return result as TResult;
        }

        protected abstract Task<TResult> DoHandleAsync( ICommandExecutionContext commandContext, T command );

        Task<object> ICommandHandler.HandleAsync( ICommandExecutionContext commandContext, object command )
        {
            return HandleAsync( commandContext, (T)command );
        }
    }

    public abstract class CommandHandler<T> : ICommandHandler<T> where T : class
    {
        public async Task<object> HandleAsync( ICommandExecutionContext commandContext, T command )
        {
            await DoHandleAsync( commandContext, command );
            return EmptyResult.Empty;
        }

        protected abstract Task DoHandleAsync( ICommandExecutionContext commandContext, T command );

        Task<object> ICommandHandler.HandleAsync( ICommandExecutionContext commandContext, object command )
        {
            return HandleAsync( commandContext, (T)command );
        }
    }

    internal class EmptyResult
    {
        public static EmptyResult Empty = new EmptyResult();
    }
}
