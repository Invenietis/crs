using CK.Crs.Tests;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using static CK.Testing.MonitorTestHelper;

namespace CK.Crs.AspNetCore.Tests
{
    [TestFixture]
    public class CrsHttpReceiverTests
    {

        [Test]
        public async Task Reads_Meta_From_Endpoint()
        {
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                crs.Meta.Commands.Should().HaveCount( 2 );
            }
        }

        [Test]
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

        [Test]
        public async Task Invoke_Command_WithError()
        {
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                var command = new WithdrawMoneyCommand { AccountId = Guid.NewGuid(), Amount = 3500M, ShouldThrow = true };

                await crs.Awaiting( x => x.InvokeCommand<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>( command ) ).Should().ThrowAsync<Exception>();

            }
        }

        [Test]
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

        [Test, Timeout( 5000 ), Explicit]
        public async Task Invoke_FireAndForget_Command_And_AutoListen_To_Callback()
        {
            Assume.That( false, "With the Microsoft.AspNetCore, the NotImplementedException in CrsEndpoint.cs above is raised..." );
            using( var crs = await CrsEndpoint.Create( "/crs" ) )
            {
                var commandResult = await crs.InvokeCommand<TransferAmountCommand, TransferAmountCommand.Result>(
                    new TransferAmountCommand { DestinationAccountId = Guid.NewGuid(), SourceAccountId = Guid.NewGuid(), Amount = 3500M } );

                // TODO: Support FAF-to-Immediate coercion in Tests.CrsEndpoint
                commandResult.Should().NotBe( null );
                commandResult.EffectiveDate.Should().NotBeBefore( DateTime.UtcNow );
                commandResult.CancellableDate.Should().NotBeAfter( DateTime.UtcNow.AddHours( 2 ) );
            }
        }
    }
}
