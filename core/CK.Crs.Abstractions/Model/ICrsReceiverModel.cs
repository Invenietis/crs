using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICrsReceiverModel
    {
        string Name { get; }

        Type ReceiverType { get; }

        string CallerIdName { get; set; }

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        bool ValidateAmbientValues { get; }

        /// <summary>
        ///  Gets wether we should apply model validation or not.
        /// </summary>
        bool ValidateModel { get; }

        /// <summary>
        /// Gets wether server side client event filtering is enabled by this endpoint.
        /// </summary>
        bool SupportsClientEventsFiltering { get; }

        IEnumerable<RequestDescription> Requests { get; }

        RequestDescription GetRequestDescription( Type requestType );
    }
}
