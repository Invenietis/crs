using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;


namespace CK.Crs.Tests
{
    public class AmountTransferredEvent
    {
        public Guid AccountId { get; internal set; }

        public decimal Amount { get; internal set; }
    }

    public class TransferAlwaysSuccessHandler : ICommandHandler<TransferAmountCommand, TransferAmountCommand.Result>
    {
        public async Task<TransferAmountCommand.Result> HandleAsync( TransferAmountCommand command, ICommandContext context )
        {
            using( context.Monitor.OpenInfo( $"Transferring {command.Amount} from {command.SourceAccountId} to {command.DestinationAccountId} " ) )
            {
                var result = new TransferAmountCommand.Result
                {
                    EffectiveDate = DateTime.UtcNow.Date.AddDays( 2 ),
                    CancellableDate = DateTime.UtcNow.AddHours( 1 )
                };
                context.Monitor.Info( $"Transfer will be effective at {result.EffectiveDate.ToString()}." );
                context.Monitor.Info( $"You have one hour to cancel it." );

                return result;
            }
        }
    }

    public class WithDrawyMoneyHandler : ICommandHandler<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>
    {
        public async Task<WithdrawMoneyCommand.Result> HandleAsync( WithdrawMoneyCommand command, ICommandContext commandContext )
        {
            if( command.ShouldThrow ) throw new InvalidOperationException( "No more money guy..." );
            var result = new WithdrawMoneyCommand.Result
            {
                Success = true
            };
            return result;
        }
    }


}
