using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using CK.Crs.Runtime;
using CK.Crs.OpenAPI.Generator;
using CK.Core;
using System.Text;
using System.Diagnostics;

namespace CK.Crs.OpenAPI
{
    class SwaggerComponent : PipelineComponent, ISwaggerProvider
    {
        readonly ISchemaRegistryFactory _schemaRegistryFactory;
        readonly SwaggerGeneratorSettings _settings;
        readonly IJsonConverter _jsonConverter;

        public SwaggerComponent( ISchemaRegistryFactory schemaRegistryFactory, SwaggerGeneratorSettings settings )
        {
            if( schemaRegistryFactory == null ) throw new ArgumentNullException( nameof( schemaRegistryFactory ) );
            if( settings == null ) throw new ArgumentNullException( nameof( settings ) );

            _schemaRegistryFactory = schemaRegistryFactory;
            _settings = settings;
            _jsonConverter = new SwaggerJsonConverter();
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Request.Path.CommandName == "swagger";
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            pipeline.Response = new SwaggerCommandResponse( CommandResponseType.Meta, GetSwagger( pipeline.Configuration ) );
            pipeline.Response.Headers.Add( "X-Meta-Type", "Swagger" );
            pipeline.Response.Headers.Add( "Content-Type", "application/json" );

            string jsonResponse = _jsonConverter.ToJson( pipeline.Response );
            var buffer = Encoding.UTF8.GetBytes(jsonResponse);
            pipeline.Response.Headers.Add( "ContentLength", buffer.Length.ToString() );

            await pipeline.Output.WriteAsync( buffer, 0, buffer.Length );
        }


        public SwaggerDocument GetSwagger( ICrsConfiguration configuration, string host = null, string[] schemes = null )
        {
            if( configuration == null ) throw new ArgumentNullException( nameof( configuration ) );

            var schemaRegistry = _schemaRegistryFactory.Create();

            var result = new SwaggerDocument();
            result.Info = new OpenAPI.Info
            {
                Title = configuration.ReceiverPath,
                Version = "1.0.0"
            };
            result.Produces = new List<string> { "application/json" };
            result.Paths = new Dictionary<string, PathItem>();
            result.Definitions = new Dictionary<string, Schema>();
            result.BasePath = CommandRoutePath.EnsureTrailingSlash( configuration.ReceiverPath );
            result.Host = host;
            result.Schemes = schemes;

            foreach( var a in configuration.Routes.All.Where( x => x.Descriptor.Traits.Contains( "Swagger" ) ) )
            {
                var operation = new Operation
                {
                    Summary = a.Descriptor.Name,
                    Description = a.Descriptor.Description,
                    Parameters = GenerateParametersFromCommandType( schemaRegistry, a.Descriptor.CommandType ),
                    Responses = GenerateResponsesFromCommandResult( configuration, schemaRegistry, a.Descriptor ),
                    Tags = a.Descriptor.Traits.Split(configuration.TraitContext.Separator),
                    Security = GenerateSecurityFromFilters( a.Filters ),
                    //Deprecated = apiDescription.IsObsolete() ? true : (bool?)null
                };
                var pathItem = new PathItem
                {
                    Post = operation
                };
                result.Paths.Add( a.Route.CommandName, pathItem );

                //var oeprationFilterContext = new OperationFilterContext(apiDescription, schemaRegistry);
                //foreach( var filter in _settings.OperationFilters )
                //{
                //    filter.Apply( operation, oeprationFilterContext );
                //}
            }
            result.Definitions = schemaRegistry.Definitions;

            //var filterContext = new DocumentFilterContext( _apiDescriptionsProvider.ApiDescriptionGroups, schemaRegistry);

            //foreach( var filter in _settings.DocumentFilters )
            //{
            //    filter.Apply( result, filterContext );
            //}

            return result;
        }



        private IList<IDictionary<string, IEnumerable<string>>> GenerateSecurityFromFilters( IReadOnlyCollection<Type> filters )
        {
            return null;
        }

        private IDictionary<string, Response> GenerateResponsesFromCommandResult(
            ICrsConfiguration configuration,
            ISchemaRegistry registry,
            CommandDescription c )
        {
            Type type = c.HandlerType;
            do
            {
                type = type.BaseType;
            }
            while( type != null &&
                (type.IsGenericType == false || (type.IsGenericType && type.GetGenericTypeDefinition() != typeof( CommandHandler<,> ))) );

            Dictionary<string, Response> responses = new Dictionary<string, Response>();
            var error = registry.GetOrRegister( typeof( CommandResponse ) );
            error.Properties["payload"] = new Schema
            {
                Type = "string"
            };
            responses.Add( "V", new Response
            {
                Description = ResponseDescriptionMap["V"],
                Schema = error
            } );
            responses.Add( "I", new Response
            {
                Description = ResponseDescriptionMap["I"],
                Schema = error
            } );
            if( type == null )
            {
                CKTrait asyncTrait = configuration.TraitContext.FindOrCreate( "Async" );
                CKTrait trait = configuration.TraitContext.FindOrCreate( c.Traits );

                if( trait.IsSupersetOf( asyncTrait ) )
                {
                    responses.Add( "A", new Response
                    {
                        Description = ResponseDescriptionMap["A"],
                        Schema = registry.GetOrRegister( typeof( CommandResponse ) )
                    } );
                    return responses;
                }

                responses.Add( "S", new Response
                {
                    Description = ResponseDescriptionMap["A"],
                    Schema = registry.GetOrRegister( typeof( CommandResponse ) )
                } );
                return responses;
            }
            Debug.Assert( type != null && type.GetGenericArguments().Length == 2 );

            var s = registry.GetOrRegister( typeof( CommandResponse ) );
            s.Properties["payload"] = registry.GetOrRegister( type.GetGenericArguments()[1] );
            responses.Add( "S", new Response
            {
                Description = ResponseDescriptionMap["S"],
                Schema = s
            } );
            return responses;
        }

        private IList<IParameter> GenerateParametersFromCommandType( ISchemaRegistry registry, Type commandType )
        {
            return new IParameter[1]
            {
                new BodyParameter
                {
                    In = "body",
                    Name = "body",
                    Required = true,
                    Description = commandType.FullName,
                    Schema = registry.GetOrRegister( commandType )
                }
            };
        }

        private static readonly Dictionary<string, string> ResponseDescriptionMap = new Dictionary<string, string>
        {
            { "V", "Validation Error" },
            { "I", "Internal Error" },
            { "S", "Synchronous" },
            { "A", "Asynchronous" },
            { "M", "Meta" }
        };
    }
}
