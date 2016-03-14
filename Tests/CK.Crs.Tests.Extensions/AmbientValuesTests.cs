using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CK.Authentication;
using Moq;
using Xunit;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using CK.Crs.Runtime;

namespace CK.Crs.Tests
{

    public class AmbientValuesTest
    {
        [Fact]
        public async Task register_provider_and_get_value()
        {
            var mockDatabase = new Mock<IAuthenticationStore>();
            mockDatabase.Setup( x => x.GetUserByName( It.IsAny<string>(), It.IsAny<CancellationToken>() ) ).Returns( Task.FromResult( new User { UserId = 12, IsEnabled = true, UserName = "john" } ) ).Verifiable();
            var sp = TestHelper.CreateServiceProvider( serviceCollection =>
            {
                var httpContextMock = new Mock<HttpContext>();
                httpContextMock.Setup( e => e.User ).Returns( () => ClaimsPrincipal.Current );

                var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
                httpContextAccessorMock.Setup( e => e.HttpContext ).Returns( httpContextMock.Object );

                serviceCollection
                    .AddSingleton<ActorIdProvider>()
                    .AddSingleton<IHttpContextAccessor>( httpContextAccessorMock.Object )
                    .AddSingleton( mockDatabase.Object );
            } );
            IAmbientValues ambientValues = new AmbientValues( new DefaultAmbientValueFactory( sp) );
            ambientValues.Register<ActorIdProvider>( "ActorId" );

            ClaimsPrincipal.ClaimsPrincipalSelector = () =>
                new ClaimsPrincipal( new ClaimsIdentity( new Claim[] { new Claim( ClaimTypes.Name, "john" ) }, "Local" ) );

            var value = await ambientValues.GetValueAsync<int>( "ActorId");

            Assert.That( value, Is.EqualTo( 12 ) );
            mockDatabase.VerifyAll();
        }
    }
}
