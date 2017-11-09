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
    public class CrsHttpReceiverTests
    {
        public ITestOutputHelper OutputHelper { get; }

        public CrsHttpReceiverTests( ITestOutputHelper outputHelper )
        {
            OutputHelper = outputHelper;
            ActivityMonitor.AutoConfiguration = m => m.Output.RegisterClient(
    new ActivityMonitorTextWriterClient( OutputHelper.WriteLine ) );

        }
        [Fact]
        public async Task Reads_Meta_From_Endpoint()
        {
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                crs.Meta.Commands.Should().HaveCount( 2 );
            }
        }

        [Fact]
        public async Task Invoke_Command()
        {
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                var command = new WithdrawMoneyCommand { AccountId = Guid.NewGuid(), Amount = 3500M };
                var commandResult = await crs.InvokeCommand<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>( command );

                commandResult.Should().NotBe( null );
                commandResult.Success.Should().BeTrue();
            }
        }

        [Fact]
        public async Task Invoke_Command_WithError()
        {
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                var command = new WithdrawMoneyCommand { AccountId = Guid.NewGuid(), Amount = 3500M, ShouldThrow = true };

                try
                {
                    await crs.InvokeCommand<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>( command );
                }
                catch( Exception ex )
                {
                    
                }
            }
        }

        [Fact]
        public async Task Invoke_FireAndForget_Command()
        {
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                var commandResult = await crs.InvokeFireAndForgetCommand(
                    new TransferAmountCommand { DestinationAccountId = Guid.NewGuid(), SourceAccountId = Guid.NewGuid(), Amount = 3500M } );

                commandResult.Should().NotBe( null );

                //commandResult.EffectiveDate.Should().NotBeBefore( DateTime.UtcNow );
                //commandResult.CancellableDate.Should().NotBeAfter( DateTime.UtcNow.AddHours( 2 ) );
            }
        }

        [Fact]
        public async Task Invoke_FireAndForget_Command_And_AutoListen_To_Callback()
        {
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                var commandResult = await crs.InvokeCommand<TransferAmountCommand, TransferAmountCommand.Result>(
                    new TransferAmountCommand { DestinationAccountId = Guid.NewGuid(), SourceAccountId = Guid.NewGuid(), Amount = 3500M } );

                commandResult.Should().NotBe( null );
                commandResult.EffectiveDate.Should().NotBeBefore( DateTime.UtcNow );
                commandResult.CancellableDate.Should().NotBeAfter( DateTime.UtcNow.AddHours( 2 ) );
            }
        }
    }
}
