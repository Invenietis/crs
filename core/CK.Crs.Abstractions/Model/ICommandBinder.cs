using System;
using System.IO;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandBinderProvider
    {
        /// <summary>
        /// Creates a <see cref="ICommandBinder"/> from the given <see cref="ICommandContext"/> and <see cref="IEndpointModel"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ICommandBinder CreateBinder( ICommandContext context, IEndpointModel model );
    }

    public interface ICommandBinder
    {
        string ContentType { get; }

        Task<object> Bind( ICommandContext commandContext );
    }
}
