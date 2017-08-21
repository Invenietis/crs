namespace CK.Crs
{
    public interface IResultStrategy
    {
        IResultReceiver GetResultReceiver( CommandModel model );
    }
}
