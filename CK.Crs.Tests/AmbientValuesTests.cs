using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CK.Authentication;
using CK.SqlServer;
using Moq;
using Xunit;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Tests
{
    public class ActorIdProvider : IAmbientValueProvider
    {
        readonly IUserTable _userTable;
        readonly IPrincipalAccessor _principalAccessor;
        readonly ISqlCallContextProvider<IDisposableSqlCallContext> _callContextProvider;

        public ActorIdProvider( IUserTable userTable, ISqlCallContextProvider<IDisposableSqlCallContext> callContextProvider, IPrincipalAccessor principalAccessor )
        {
            _userTable = userTable;
            _principalAccessor = principalAccessor;
            _callContextProvider = callContextProvider;
        }

        public string Name
        {
            get { return "ActorId"; }
        }

        public const int Default = 0;

        public async Task<object> GetValueAsync( IAmbientValues values )
        {
            var currentIdentity = _principalAccessor.User.Identity;
            if( currentIdentity.IsAuthenticated )
            {
                using( var sqlContext = _callContextProvider.Acquire() )
                {
                    return await _userTable.GetIdByName( sqlContext.Executor, currentIdentity.Name );
                }
            }

            return Default;
        }
    }

    public class AmbientValuesTest
    {
        [Fact]
        public async Task register_provider_and_get_value()
        {
            var mockDatabase = new Mock<IUserTable>();
            mockDatabase.Setup( x => x.GetIdByName( It.IsAny<ISqlCommandExecutor>(), It.IsAny<string>() ) ).Returns( Task.FromResult<int>(12)).Verifiable();
            var sp = TestHelper.CreateServiceProvider( serviceCollection =>
            {
                serviceCollection
                    .AddSingleton<ISqlCallContextProvider<IDisposableSqlCallContext>, SqlStandardCallContextProvider>()
                    .AddSingleton<ActorIdProvider>()
                    .AddInstance<IUserTable>( mockDatabase.Object )
                    .AddInstance<IPrincipalAccessor>( new TestPrincipalAccessor() );
            } );
            IAmbientValues ambientValues = new AmbientValues( new DefaultAmbientValueFactory( sp) );
            ambientValues.Register<ActorIdProvider>( "ActorId" );
             
            ClaimsPrincipal.ClaimsPrincipalSelector = () =>
                new ClaimsPrincipal( new ClaimsIdentity( new Claim[] { new Claim( ClaimTypes.Name, "john" ) }, "Local" ));
             
            var value = await ambientValues.GetValueAsync<int>( "ActorId");

            Assert.That( value, Is.EqualTo( 12 ) );
            mockDatabase.VerifyAll();
        }
    }
}
