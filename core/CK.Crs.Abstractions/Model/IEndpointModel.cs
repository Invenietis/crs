using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface IEndpointModel
    {
        string Name { get; }

        Type EndpointType { get; }

        /// <summary>
        /// Defaults to CallerId
        /// </summary>
        string CallerIdName { get; set; }

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
