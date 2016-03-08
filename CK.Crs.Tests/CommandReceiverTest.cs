//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;
//using CK.Crs;
//using CK.Crs.Tests.Fake;
//using Microsoft.Extensions.DependencyInjection;
//using CK.Core;
//using CK.Crs.Tests.Handlers;
//using Microsoft.Owin;
//using CK.Crs.Runtime;
//using Xunit;
//using Assert = NUnit.Framework.Assert;
//using Is = NUnit.Framework.Is;
//using Microsoft.AspNetCore.Http;

//namespace CK.Crs.Tests
//{
//    // This project can output the Class library as a NuGet Package.
//    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
//    public class CommandReceiverTest
//    {
//        public CommandReceiverTest()
//        {
//            ApplicationServices = TestHelper.CreateServiceProvider( services =>
//            {
//                services.AddCommandReceiver( c =>
//                {
//                    c.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>();
//                } );
//                services.AddSingleton<ICommandResponseDispatcher, EventChannel>();
//            } );
//        }

//        public IServiceProvider ApplicationServices { get; private set; }

//        public RequestDelegate SuccessDelegate { get; } = new RequestDelegate( ( HttpContext httpContext ) =>
//        {
//            return Task.FromResult<object>( null );
//        } );

//        class ShouldNotInvokeMiddleware : OwinMiddleware
//        {
//            public ShouldNotInvokeMiddleware( OwinMiddleware next ) : base( next )
//            {
//            }

//            public override Task Invoke( IOwinContext context )
//            {
//                throw new CKException( "Next delegate invoked." );
//            }
//        }
//        class ShouldInvokeMiddleware : OwinMiddleware
//        {
//            public ShouldInvokeMiddleware( OwinMiddleware next ) : base( next )
//            {
//            }

//            public override Task Invoke( IOwinContext context )
//            {
//                return Task.FromResult<object>( null );
//            }
//        }

//        [Fact]
//        public async Task CommandReceiver_Should_Be_Scoped_To_A_Route_Prefix()
//        {
//            var cmd =new Handlers.TransferAmountCommand
//            {
//                SourceAccountId = Guid.NewGuid(),
//                DestinationAccountId = Guid.NewGuid(),
//                Amount = 1000
//            };

//            var prefix = "/api";

//            var routes = new CommandRouteCollection( prefix);
//            {
//                var middleWare = new CommandReceiverMiddleware( new ShouldNotInvokeMiddleware(null), routes, ApplicationServices.GetRequiredService<ICommandReceiver>(), ApplicationServices.GetRequiredService<ICommandFormatter>() );
//                using( var httpContext = new FakeHttpContext( ApplicationServices, "/api", SerializeRequestBody( cmd ) ) )
//                {
//                    try
//                    {
//                        await middleWare.Invoke( new OwinContext() );
//                    }
//                    catch( Exception ex )
//                    {
//                        Assert.That( ex, Is.InstanceOf<CKException>() );
//                        Assert.That( ex.Message, Is.EqualTo( "Next delegate invoked." ) );
//                    }
//                }

//                routes.AddCommandRoute( new CommandDescriptor
//                {
//                    CommandType = typeof( TransferAmountCommand ),
//                    HandlerType = typeof( TransferAlwaysSuccessHandler ),
//                    IsLongRunning = false,
//                    Name = typeof( TransferAmountCommand ).Name
//                } );
//                using( var httpContext = new FakeHttpContext( ApplicationServices, "/api", SerializeRequestBody( cmd ) ) )
//                {
//                    try
//                    {
//                        await middleWare.Invoke( new OwinContext() );
//                    }
//                    catch( Exception ex )
//                    {
//                        Assert.That( ex, Is.InstanceOf<CKException>() );
//                        Assert.That( ex.Message, Is.EqualTo( "Next delegate invoked." ) );
//                    }
//                }
//            }
//            {
//                var middleWare = new CommandReceiverMiddleware( new ShouldNotInvokeMiddleware(null), routes, ApplicationServices );

//                using( var httpContext = new FakeHttpContext( ApplicationServices, "/api/TransferAmountCommand", SerializeRequestBody( cmd ) ) )
//                {
//                    await middleWare.Invoke( new OwinContext() );
//                    Assert.NotNull( httpContext.Response.Body );
//                    Assert.True( httpContext.Response.Body.Length > 0 );
//                }
//            }
//        }


//        private Stream SerializeRequestBody<T>( T command )
//        {
//            Stream s = new MemoryStream();
//            StreamWriter sw = new StreamWriter(s);
//            Newtonsoft.Json.JsonSerializer.Create().Serialize( sw, command );
//            s.Seek( 0, SeekOrigin.Begin );
//            return s;
//        }
//    }

//}
