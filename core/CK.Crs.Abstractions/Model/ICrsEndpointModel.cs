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


        IReadOnlyList<RequestDescription> Requests { get; }
    }
}
