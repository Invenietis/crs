using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface IEndpointModel
    {
        string Path { get; }

        /// <summary>
        /// Gets the default <see cref="ICommandBinder"/> associates with this endpoint
        /// </summary>
        Type Binder { get; }

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

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        bool ApplyAmbientValuesValidation { get; }

        /// <summary>
        ///  Gets wether we should apply model validation or not.
        /// </summary>
        bool ApplyModelValidation { get; }

        IEnumerable<CommandModel> Commands { get; }

        CommandModel GetCommandModel( Type commandType );
    }
}
