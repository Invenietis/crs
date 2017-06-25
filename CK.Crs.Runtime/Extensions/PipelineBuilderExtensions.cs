using CK.Crs.Runtime;
using CK.Core;
using System;
using CK.Crs.Runtime.Formatting;

namespace CK.Crs
{
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Build the command from the CommandRequest data.
        /// </summary>
        static public IPipelineBuilder UseJsonCommandBuilder(this IPipelineBuilder builder)
        {
            return builder.Use<JsonCommandBuilder>();
        }

        /// <summary>
        /// Build the command from the CommandRequest data.
        /// </summary>
        static public IPipelineBuilder UseJsonCommandWriter(this IPipelineBuilder builder)
        {
            return builder.Use<JsonCommandWriter>();
        }
    }
}
