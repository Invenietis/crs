using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICrsEndpointModel
    {
        string Name { get; }

        Type EndpointType { get; }

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        bool ValidateAmbientValues { get; }

        /// <summary>
        ///  Gets wether we should apply model validation or not.
        /// </summary>
        bool ValidateModel { get; }

        IReadOnlyList<RequestDescription> Requests { get; }

        RequestDescription GetRequestDescription( Type requestType );
    }
}
