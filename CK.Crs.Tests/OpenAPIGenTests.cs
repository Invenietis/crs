using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.OpenAPI;
using CK.Crs.OpenAPI.Generator;
using CK.Crs.Runtime;
using Moq;
using NUnit.Framework;

namespace CK.Crs.Tests
{
    public class OpenAPIGenTests
    {
        protected ICrsConfiguration PrepareCrsEndpoint( string basePath, params CommandRoute[] routes )
        {
            var mockRoutes = new Mock<ICommandRouteCollection>();
            mockRoutes.Setup( e => e.All ).Returns( routes );

            var mockConf = new Mock<ICrsConfiguration>();
            mockConf.Setup( e => e.ReceiverPath ).Returns( basePath );
            mockConf.Setup( e => e.Routes ).Returns( mockRoutes.Object );
            mockConf.Setup( e => e.TraitContext ).Returns( new CKTraitContext( "TestContext" ) );

            ICrsConfiguration conf = mockConf.Object;

            return conf;
        }

        [Test]
        public void no_routes_should_return_minimalist_swagger_document()
        {
            ICrsConfiguration config = PrepareCrsEndpoint( "api/v1/tests" );
            ISchemaRegistryFactory factory = CreateSchemaRegistryFactory();
            ISwaggerProvider swaggerProvider = new SwaggerComponent( factory, new SwaggerGeneratorSettings
            {
                DescribeAllParametersInCamelCase = true,
                IgnoreObsoleteActions = true
            });
            SwaggerDocument document = swaggerProvider.GetSwagger( config );

            Assert.That( document.BasePath, Is.EqualTo( "api/v1/tests" ) );
            Assert.That( document.Paths, Has.Count.EqualTo( 0 ) );
            Assert.That( document.Definitions, Has.Count.EqualTo( 0 ) );
        }

        [Test]
        public void single_route_single_definition_no_response()
        {
            ICrsConfiguration config = PrepareCrsEndpoint(
                "api/v1/tests",
                new CommandRoute( "api/v1/tests/createUser", new CommandDescription( "createUser", typeof( CreateUserCommand ),  typeof( UserCommandHandler ) )
                {
                    Description = "Create a user",
                    Traits = "Swagger"
                }
            ));
            ISchemaRegistryFactory factory = CreateSchemaRegistryFactory();
            ISwaggerProvider swaggerProvider = new SwaggerComponent( factory, new SwaggerGeneratorSettings
            {
                DescribeAllParametersInCamelCase = true,
                IgnoreObsoleteActions = true
            });
            SwaggerDocument document = swaggerProvider.GetSwagger( config);

            Assert.That( document.BasePath, Is.EqualTo( "api/v1/tests" ) );
            Assert.That( document.Paths, Has.Count.EqualTo( 1 ) );
            Assert.That( document.Definitions, Has.Count.EqualTo( 1 ) );

            Assert.That( document.Paths.All( o => o.Value.Get == null ), Is.True );
            Assert.That( document.Paths.All( o => o.Value.Put == null ), Is.True );
            Assert.That( document.Paths.All( o => o.Value.Delete == null ), Is.True );
            Assert.That( document.Paths.All( o => o.Value.Post != null ), Is.True );

            Assert.That( document.Paths["createUser"], Is.Not.Null );
            Assert.That( document.Paths["createUser"].Post.Parameters, Has.Exactly( 1 ).InstanceOf<BodyParameter>(), "Only one parameter by default, the BodyParameter" );
            Assert.That( ((BodyParameter)document.Paths["createUser"].Post.Parameters[0]).Schema, Is.Not.Null );


            Assert.That( document.Definitions, Has.Exactly( 1 ).InstanceOf<KeyValuePair<string,Schema>>() );
            Assert.That( document.Definitions["CreateUserCommand"], Is.Not.Null );
            Assert.That( document.Definitions["CreateUserCommand"].Type, Is.EqualTo( "object" ) );

        }

        private static ISchemaRegistryFactory CreateSchemaRegistryFactory()
        {
            ISchemaRegistryFactory schemaRegistryFactory = new SchemaRegistryFactory( new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            }, new SchemaRegistrySettings
            {
                IgnoreObsoleteProperties = true,
                DescribeAllEnumsAsStrings = true,
                DescribeStringEnumsInCamelCase = false
            } );

            return schemaRegistryFactory;
        }
    }
}
