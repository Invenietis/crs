using System;
using System.IO;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandBinder
    {
        /// <summary>
        /// The content type this command binder supports.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Attemsp to bind the given <paramref name="commandContext"/> to an instance of an object.
        /// Binders can relies on features exposed by the <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        Task<object> Bind( ICommandContext commandContext );
    }
}
