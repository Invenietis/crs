using CK.Crs.Runtime;
using CK.Crs.Runtime.Pipeline;

namespace CK.Crs
{
    public static class PipelineBuilderExtensions
    {
        public static IPipelineBuilder UseDefault( this IPipelineBuilder builder )
        {
            return builder
                .Clear()
                .UseCommandRouter()
                .UseJsonCommandBuilder()
                .UseAmbientValuesValidator()
                .UseFilters()
                .UseCommandExecutor();
        }

        public static IPipelineBuilder Use<T>( this IPipelineBuilder builder ) where T : PipelineComponent
        {
            return builder.Use( pipeline => builder.CreateComponent<T>( pipeline ).TryInvoke( pipeline.CancellationToken ) );
        }

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

        /// <summary>
        /// Command execution step
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        static public IPipelineBuilder UseCommandExecutor( this IPipelineBuilder builder )
        {
            return builder.Use<CommandExecutor>();
        }
    }
}
