using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// Defines a crs endpoint
    /// </summary>
    public interface ICrsEndpoint
    {
        /// <summary>
        /// Creates a <see cref="ICrsEndpointPipeline"/>.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="endpointModel"></param>
        /// <returns><see cref=ICrsEndpointPipeline"/></returns>
        ICrsEndpointPipeline CreatePipeline( IActivityMonitor monitor, IEndpointModel endpointModel );
    }
}
