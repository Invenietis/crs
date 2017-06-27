using CK.Crs.Runtime;
using CK.Core;
using System;
using CK.Crs.Runtime.Filtering;
using CK.Crs.Runtime.Routing;
using CK.Crs.Runtime.Formatting;
using CK.Crs.Runtime.Meta;

namespace CK.Crs
{
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Looks up the route collection for a registered command.
        /// </summary>
        /// <param name="builder"></param>
        static public IPipelineBuilder UseCommandRouter( this IPipelineBuilder builder, ICommandRouteCollection routes )
        {
            return builder.Use<CommandRouter>(routes);
        }


        /// <summary>
        /// Check the ambient values of the command parameters.
        /// </summary>
        static public IPipelineBuilder UseAmbientValuesValidator( this IPipelineBuilder builder )
        {
            return builder.Use<AmbientValuesValidator>();
        }

        /// <summary>
        /// Build the command from the CommandRequest data.
        /// </summary>
        static public IPipelineBuilder UseMetaComponent( this IPipelineBuilder builder, ICommandRouteCollection routes )
        {
            return builder.Use<MetaComponent>( routes );
        }


        /// <summary>
        /// Invoke command filters. 
        /// </summary>
        static public IPipelineBuilder UseFilters( this IPipelineBuilder builder, ICommandRouteCollection routes)
        {
            return builder.Use<CommandFiltersInvoker>(routes);
        }
    }
}
