namespace CK.Infrastructure.Commands
{
    public interface ICommandRunnerFactory
    {
        ICommandRunner CreateRunner( CommandContext runtimeContext );
    }
}
