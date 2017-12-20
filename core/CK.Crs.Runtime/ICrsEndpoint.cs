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
        /// <param name="endpointModel"></param>
        /// <returns></returns>
        ICrsEndpointPipeline CreatePipeline( IEndpointModel endpointModel );
    }
}
