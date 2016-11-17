using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Crs.OpenAPI;
using CK.Crs.OpenAPI.Generator;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Looks up the route collection for a registered command.
        /// </summary>
        /// <param name="builder"></param>
        static public IPipelineBuilder UseSwagger( this IPipelineBuilder builder )
        {
            return builder.Use<SwaggerComponent>();
        }

        static public void AddSwagger( this IServiceCollection services )
        {
            services.AddSingleton<ISchemaRegistryFactory>( new SchemaRegistryFactory( new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            }, new SchemaRegistrySettings
            {
                IgnoreObsoleteProperties = true,
                DescribeAllEnumsAsStrings = true,
                DescribeStringEnumsInCamelCase = false
            } ) );

            services.AddSingleton( new SwaggerGeneratorSettings
            {
                DescribeAllParametersInCamelCase = true,
                IgnoreObsoleteActions = true
            } );
        }
    }
}
