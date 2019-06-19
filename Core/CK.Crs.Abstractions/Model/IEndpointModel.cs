using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface IEndpointModel : IBindable, IFilterable
    {
        string Path { get; }

        /// <summary>
        /// Gets the response formatter used by this endpoint.
        /// </summary>
        IResponseFormatter ResponseFormatter { get; }

        /// <summary>
        /// Gets the gloabl <see cref="ICrsModel"/> this endpoint belongs to.
        /// </summary>
        ICrsModel CrsModel { get; }

        /// <summary>
        /// Defaults to CallerId
        /// </summary>
        string CallerIdName { get; }

        IEnumerable<ICommandModel> Commands { get; }

        ICommandModel GetCommandModel( Type commandType );
    }
}
