namespace CK.Infrastructure.Commands
{
    public interface IDecorator
    {
        int Order { get; }

        void OnCommandExecuting( CommandExecutionContext ctx );

        void OnException( CommandExecutionContext ctx );

        void OnCommandExecuted( CommandExecutionContext ctx );
    }
}