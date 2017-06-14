using System;
using System.Collections.Generic;


namespace CK.Crs.OpenAPI.Generator
{
    public class SwaggerGeneratorSettings
    {
        public SwaggerGeneratorSettings() : this( "swagger" )
        {
        }

        public SwaggerGeneratorSettings( string swaggerPath )
        {
            SwaggerPath = swaggerPath;
            SwaggerDocs = new Dictionary<string, SwaggerDocumentDescriptor>();
            TagComparer = Comparer<string>.Default;
            SecurityDefinitions = new Dictionary<string, SecurityScheme>();
        }

        /// <summary>
        /// The path to the swagger description
        /// </summary>
        public string SwaggerPath { get; private set; }

        public IDictionary<string, SwaggerDocumentDescriptor> SwaggerDocs { get; private set; }

        public bool IgnoreObsoleteCommands { get; set; }

        public IComparer<string> TagComparer { get; set; }

        public bool DescribeAllParametersInCamelCase { get; set; }

        public IDictionary<string, SecurityScheme> SecurityDefinitions { get; private set; }

        internal SwaggerGeneratorSettings Clone()
        {
            return new SwaggerGeneratorSettings
            {
                SwaggerDocs = SwaggerDocs,
                IgnoreObsoleteCommands = IgnoreObsoleteCommands,
                TagComparer = TagComparer,
                SecurityDefinitions = SecurityDefinitions,
            };
        }
    }

    public class SwaggerDocumentDescriptor
    {
        public SwaggerDocumentDescriptor( Info info )
        {
            Info = info;
        }

        public Info Info { get; private set; }
    }
}