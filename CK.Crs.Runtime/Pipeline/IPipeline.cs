using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    public interface IPipeline : IDisposable
    {
        /// <summary>
        /// The request this pipeline is originated from.
        /// </summary>
        CommandRequest Request { get; }
        /// <summary>
        /// The resulting command action.
        /// </summary>
        CommandAction Action { get; }
        /// <summary>
        /// The response creating by the invokation of the action
        /// </summary>
        CommandResponse Response { get; set; }
        /// <summary>
        /// An <see cref="IActivityMonitor"/> that monitors operations of the pipeline.
        /// </summary>
        IActivityMonitor Monitor { get; }
        /// <summary>
        /// Raises events during the command processing at any stage of the pipeline.
        /// </summary>
        PipelineEvents Events { get; }
    }

}
