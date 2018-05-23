using Microsoft.Owin;

namespace CK.Crs.Owin
{
    public interface IOwinContextCommandFeature
    {
        /// <summary>
        /// Sets the <paramref name="OwinContext"/>. This makes the object lifetime dependent of the request.
        /// </summary>
        /// <param name="OwinContext"></param>
        IOwinContext OwinContext { get; }
    }
}
