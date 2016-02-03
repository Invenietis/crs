namespace CK.Infrastructure.Commands
{
    public interface ICommandRegistration
    {
        ICommandRegistration AddDecorator<T>() where T : ICommandDecorator;
        ICommandRegistration CommandName( string commandName );
        ICommandRegistration IsLongRunning();
    }
}