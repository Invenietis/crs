namespace CK.Infrastructure.Commands
{
    public interface ICommandDecorator
    {
        int Order { get; }

        void OnCommandExecuting( CommandExecutionContext ctx );

        void OnException( CommandExecutionContext ctx );

        void OnCommandExecuted( CommandExecutionContext ctx );
    }
}