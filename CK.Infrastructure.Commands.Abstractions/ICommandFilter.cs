namespace CK.Infrastructure.Commands
{
    public interface ICommandFilter
    {
        int Order { get; }
        void OnCommandReceived( CommandExecutionContext executionContext );
    }
}