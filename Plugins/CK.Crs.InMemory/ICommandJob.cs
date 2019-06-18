namespace CK.Crs.InMemory
{
    public interface ICommandJob
    {
        ICommandContext CommandContext { get;}
        object Command { get;}
    }
}
