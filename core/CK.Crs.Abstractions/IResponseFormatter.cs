namespace CK.Crs
{
    /// <summary>
    /// Defines a <see cref="Response"/> formatter which is able to convert a <see cref="Response"/> to a string
    /// </summary>
    public interface IResponseFormatter
    {
        string ContentType { get; }
        string Format( Response response );
    }
}
