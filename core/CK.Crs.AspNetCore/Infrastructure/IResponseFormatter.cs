namespace CK.Crs
{
    public interface IResponseFormatter
    {
        string ContentType { get; }
        string Format( Response response );
    }
}
