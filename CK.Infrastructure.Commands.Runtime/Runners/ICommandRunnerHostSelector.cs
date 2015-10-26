namespace CK.Infrastructure.Commands
{
    public interface ICommandRunnerHostSelector
    {
        ICommandRunnerHost SelectHost( CommandContext runtimeContext );
    }
}
