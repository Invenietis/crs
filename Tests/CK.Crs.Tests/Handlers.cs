using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Tests.Handlers
{
    public class AmountTransferredEvent
    {
        public Guid AccountId { get; internal set; }

        public decimal Amount { get; internal set; }
    }

    public class TransferAlwaysSuccessHandler : CommandHandler<TransferAmountCommand, TransferAmountCommand.Result>
    {
        protected override Task<TransferAmountCommand.Result> DoHandleAsync( ICommandExecutionContext context, TransferAmountCommand command )
        {
            using( context.Monitor.OpenInfo().Send( $"Transferring {command.Amount} from {command.SourceAccountId} to {command.DestinationAccountId} " ) )
            {
                var result = new TransferAmountCommand.Result
                {
                    EffectiveDate = DateTime.UtcNow.Date.AddDays( 2 ),
                    CancellableDate = DateTime.UtcNow.AddHours( 1 )
                };
                context.Monitor.Info().Send( $"Transfer will be effective at {result.EffectiveDate.ToString()}." );
                context.Monitor.Info().Send( $"You have one hour to cancel it." );

                context.ExternalEvents.Push( new AmountTransferredEvent
                {
                    AccountId = command.DestinationAccountId,
                    Amount = command.Amount
                } );

                context.ExternalEvents.ForcePush( new AmountTransferredEvent
                {
                    AccountId = command.DestinationAccountId,
                    Amount = command.Amount
                } );

                result.OperationId = context.Scheduler.Schedule( new PerformTransferAmountCommand
                {
                    Amount = command.Amount,
                    DestinationAccountId = command.DestinationAccountId,
                    SourceAccountId = command.SourceAccountId
                }, new CommandSchedulingOption( result.CancellableDate, true ) );

                return Task.FromResult( result );
            }
        }
    }


    public class TransferAlwaysSuccessHandlerWithDecoration : TransferAlwaysSuccessHandler
    {
    }

    public class WithDrawyMoneyHandler : CommandHandler<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>
    {
        protected override Task<WithdrawMoneyCommand.Result> DoHandleAsync( ICommandExecutionContext commandContext, WithdrawMoneyCommand command )
        {
            var result =  new WithdrawMoneyCommand.Result
            {
                Success = true
            };
            return Task.FromResult( result );
        }
    }

}
