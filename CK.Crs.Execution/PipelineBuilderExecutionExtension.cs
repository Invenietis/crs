using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Execution;

namespace CK.Crs
{
    public static class PipelineBuilderExecutionExtension
    {
        /// <summary>
        /// Command execution step
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        static public IPipelineBuilder UseSyncCommandExecutor( this IPipelineBuilder builder )
        {
            return builder.Use<SyncCommandExecutor>();
        }
        /// <summary>
        /// Command execution step
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        static public IPipelineBuilder UseTaskBasedCommandExecutor( this IPipelineBuilder builder )
        {
            return builder.Use<TaskBasedCommandExecutor>();
        }

    }
}
