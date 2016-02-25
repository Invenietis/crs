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
            var sp = TestHelper.CreateServiceProvider( serviceCollection =>
            {
                serviceCollection
                    .AddSingleton<ActorIdProvider>()
                    .AddSingleton<IUserTable>()
                    .AddSingleton<IPrincipalAccessor>();
            } );
            IAmbientValues ambientValues = new AmbientValues( new DefaultAmbientValueFactory( sp) );
            ambientValues.Register<ActorIdProvider>( "ActorId" );

            var principal = new ClaimsPrincipal( new ClaimsIdentity( new Claim[] { new Claim( ClaimTypes.Name, "john" ) } ));
            var mockDatabase = new Mock<IUserTable>()
                .Setup( x => x.GetIdByName( It.IsAny<ISqlCommandExecutor>(), It.IsAny<string>() ) ).Returns( Task.FromResult<int>(12));
            //.Setup( x => x.Zones.GetZoneIdByHostNameAsync( It.IsAny<string>() ) ).Returns( Task.FromResult<int>(0))
            //.Setup( x => x.Cultures.GetXLCIDByCultureNameAsync( It.IsAny<string>() ) ).Returns( Task.FromResult<int>(9));


            var value = await ambientValues.GetValueAsync( "ActorId");

            Assert.That( value, Is.EqualTo( 12 ) );
        }
    }
}
