using CK.Crs.Runtime;
using CK.Core;
using System;
using Microsoft.Extensions.DependencyInjection;
using CK.Crs.Runtime.Filtering;
using CK.Crs.Runtime.Routing;
using CK.Crs.Runtime.Formatting;
using CK.Crs.Pipeline;

namespace CK.Crs
{
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Looks up the route collection for a registered command.
        /// </summary>
        /// <param name="routes"></param>
        static public IPipelineBuilder UseCommandRouter( this IPipelineBuilder builder )
        {
            return builder.Use<CommandRouter>();
        }

        /// <summary>
        /// Build the command from the CommandRequest data.
        /// </summary>
        static public IPipelineBuilder UseJsonCommandBuilder( this IPipelineBuilder builder )
        {
            return builder.Use<JsonCommandBuilder>();
        }

        /// <summary>
        /// Build the command from the CommandRequest data.
        /// </summary>
        static public IPipelineBuilder UseJsonResponseWriter( this IPipelineBuilder builder )
        {
            return builder.Use<JsonCommandResponseWriter>();
        }

        /// <summary>
        /// Check the ambient values of the command parameters.
        /// </summary>
        static public IPipelineBuilder UseAmbientValuesValidator( this IPipelineBuilder builder )
        {
            return builder.Use<AmbientValuesValidator>();
        }

        /// <summary>
        /// Invoke command filters. 
        /// </summary>
        static public IPipelineBuilder UseFilters( this IPipelineBuilder builder )
        {
            return builder.Use<CommandFiltersInvoker>();
        }

        static public IPipelineBuilder UseDefault( this IPipelineBuilder builder )
        {
            return builder
                .Clear()
                .UseCommandRouter()
                .UseJsonCommandBuilder()
                .UseAmbientValuesValidator()
                .UseFilters();
        }
    }
}
