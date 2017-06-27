using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
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
        static public IPipelineBuilder UseDefaultCommandExecutor( this IPipelineBuilder builder )
        {
            return builder.Use<SyncCommandExecutor>();
        }

        /// <summary>
        /// Command execution step
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        static public IPipelineBuilder UseTaskBasedCommandExecutor( this IPipelineBuilder builder, CKTraitContext traitContext )
        {
            return builder.Use<TaskBasedCommandExecutor>(traitContext.FindOrCreate(TaskBasedCommandExecutor.Trait));
        }

    }
}
