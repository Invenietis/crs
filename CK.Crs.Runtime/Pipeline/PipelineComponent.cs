using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public abstract class PipelineComponent
    {
        /// <summary>
        /// Implementations should tell if the component can be invoked regarding the state of the <see cref="IPipeline"/>.
        /// </summary>
        /// <param name="pipeline">The current execution <see cref="IPipeline"/></param>
        /// <returns>True if the component can be invoked, false otherwise.</returns>
        public abstract bool ShouldInvoke( IPipeline pipeline );

        /// <summary>
        /// Helper that will only call Invoke if and only if ShouldInvoke is true. Otherwise an Trace log is created.
        /// </summary>
        /// <param name="pipeline">The current execution <see cref="IPipeline"/></param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task<IPipeline> TryInvoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            if( ShouldInvoke( pipeline ) ) await Invoke( pipeline );
            else
            {
                pipeline.Monitor.Trace().Send( "Component {0} should not been invoked. Condition failed.", GetType().Name );
            }
            return pipeline;
        }

        /// <summary>
        /// Implementations should implement ther own logic of processing to mutate the given <see cref="IPipeline"/>.
        /// </summary>
        /// <remarks>
        /// Any components can at any time creates a <see cref="CommandResponse"/> and set it to the current <see cref="IPipeline"/>.
        /// </remarks>
        /// <param name="pipeline">The current execution <see cref="IPipeline"/></param>
        /// <param name="token">The cancellation token that should be honor.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public abstract Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) );
    }
}
