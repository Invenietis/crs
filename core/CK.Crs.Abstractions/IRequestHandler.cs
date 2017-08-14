namespace CK.Crs
{
    public interface IRequestHandler
    {
    }

    public interface ICommandHandler : IRequestHandler
    {
    }


    public interface IEventHandler : IRequestHandler
    {
    }
}
