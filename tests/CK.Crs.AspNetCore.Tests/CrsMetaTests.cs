using CK.AspNet.Tester;
using CK.Core;
using CK.Crs.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CK.Crs.AspNetCore.Tests
{
    public class CrsMetaTests
    {
        public ITestOutputHelper OutputHelper { get; }

        public CrsMetaTests( ITestOutputHelper outputHelper )
        {
            OutputHelper = outputHelper;
            ActivityMonitor.AutoConfiguration = m => m.Output.RegisterClient(
    new ActivityMonitorTextWriterClient( OutputHelper.WriteLine ) );

        }
        [Fact]
        public async Task Reads_Meta_From_Endpoint()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            using( var server = new TestServer( builder ) )
            using( var client = new TestServerClient( server, true ) )
            {
                var result = await client.GetMeta( "/crs" );
                result.Commands.Should().HaveCount( 2 );
            }
        }

        [Fact]
        public async Task Invoke_Command()
        {

            var builder = new WebHostBuilder().UseStartup<Startup>();
            using( var server = new TestServer( builder ) )
            using( var client = new TestServerClient( server, true ) )
            {
                var result = await client.GetMeta( "/crs" );
                var command = result.Commands.FirstOrDefault( c => c.Value.CommandType.Contains( "WithdrawMoneyCommand" ) );
                var commandName = command.Key;
                var commandModel = command.Value;

                var commandResult = await client.InvokeCommand<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>(
                    "/crs",
                    new WithdrawMoneyCommand { AccountId = Guid.NewGuid(), Amount = 3500M },
                    commandModel );

                commandResult.Should().NotBe( null );
                commandResult.Success.Should().BeTrue();
            }
        }
    }
}
