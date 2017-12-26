using CK.Core;
using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICrsEndpointPipeline : IDisposable
    {
        IActivityMonitor Monitor { get; }

        /// <summary>
        /// Gets whether the current pipeline is valid or not.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <returns></returns>
        Task<Response> ProcessCommand();
    }
}
