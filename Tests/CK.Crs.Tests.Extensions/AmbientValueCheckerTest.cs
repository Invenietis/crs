using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using CK.Crs.Runtime;
using CK.Authentication;
using Xunit.Abstractions;
using CK.Crs.Runtime.Pipeline;
using Microsoft.AspNetCore.Http;
using CK.Core;

namespace CK.Crs.Tests
{
    public abstract class Command
    {
        public int ActorId { get; set; }
    }

    public class AmbientValueCheckerTest
    {
        class SimpleCommand : Command
        {
            public string Data { get; set; }
        }

        ITestOutputHelper _output;
        public AmbientValueCheckerTest( ITestOutputHelper output )
        {
            _output = output;
        }

        [Theory]
        [InlineData( "john", 12, false )]
        [InlineData( "john", 11, true )]
        [InlineData( "john", 0, true )]
        [InlineData( "john", 1, true )]
        public async Task AmbientValueActorId_Validation_Cases( string userName, int ambientValueActorId, bool shouldBeRejected )
        {
            ClaimsPrincipal.ClaimsPrincipalSelector = () =>
                new ClaimsPrincipal( new ClaimsIdentity( new Claim[] { new Claim( ClaimTypes.Name, userName ) }, "Local" ) );

            var authenticationStore = new Mock<IAuthenticationStore>();

            var sp = TestHelper.CreateServiceProvider( serviceCollection =>
            {
                var httpContextMock = new Mock<HttpContext>();
                httpContextMock.Setup( e => e.User ).Returns( () => ClaimsPrincipal.Current);

                var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
                httpContextAccessorMock.Setup( e => e.HttpContext ).Returns( httpContextMock.Object );

                authenticationStore
                    .Setup( e => e.GetUserByName( It.IsAny<string>(), It.IsAny<CancellationToken>()) )
                    .Returns( () => Task.FromResult( new User
                    {
                        UserId = ambientValueActorId,
                        UserName = userName,
                        IsEnabled = true
                    }))
                    .Verifiable();

                serviceCollection
                    .AddSingleton<ActorIdProvider>()
                    .AddSingleton<IHttpContextAccessor>( httpContextAccessorMock.Object )
                    .AddSingleton<IAuthenticationStore>( authenticationStore.Object);
            } );

            var command = new SimpleCommand
            {
                ActorId = 12
            };

            var pipelineConfig = new Mock<IPipelineConfiguration>();

            var pipelineEvents = new PipelineEvents();

            pipelineEvents.AmbientValuesValidating = ( validationContext ) =>
            {
                validationContext.Monitor.Trace().Send( " ------ FROM PIPELINE - ValidatingAmbientValues ------" );
                validationContext.Monitor.Trace().Send( validationContext.RejectReason ?? "No Reason" );
                Assert.That( validationContext.Rejected, Is.False );
                return Task.FromResult( 0 );
            };
            pipelineEvents.AmbientValuesValidated = ( validationContext ) =>
            {
                validationContext.Monitor.Trace().Send( " ------ FROM PIPELINE - AmbientValuesValidated ------" );
                validationContext.Monitor.Trace().Send( validationContext.RejectReason ?? "No Reason" );
                Assert.That( validationContext.Rejected, Is.EqualTo( shouldBeRejected ) );
                return Task.FromResult( 0 );
            };
            pipelineConfig.SetupGet( e => e.Events ).Returns( pipelineEvents );

            var context = TestHelper.CreateContext( command, _output);

            var fakePipeline = new Mock<IPipeline>();
            fakePipeline.SetupGet( e => e.Action ).Returns( context.Action );
            fakePipeline.SetupGet( e => e.Monitor ).Returns( context.Monitor );
            fakePipeline.SetupGet( e => e.Configuration ).Returns( pipelineConfig.Object );

            // Act
            var ambientValues = TestHelper.CreateAmbientValues( sp, context.Monitor );
            ambientValues.Register<ActorIdProvider>( "ActorId" );

            var validator = new AmbientValuesValidator(  ambientValues );
            await validator.Invoke( fakePipeline.Object, default( CancellationToken ) );

            // Assert
            authenticationStore.VerifyAll();
        }
    }
}