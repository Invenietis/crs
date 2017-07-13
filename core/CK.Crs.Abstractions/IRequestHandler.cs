namespace CK.Crs
{
    public interface IRequestHandler
    {
    }

    public interface ICommandHandler : IRequestHandler
    {
    }

    public interface ICommandHandlerWithResult : ICommandHandler
    {
    }

    public interface IEventHandler : IRequestHandler
    {
    }
}
