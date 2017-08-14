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

    public class TransferAlwaysSuccessHandler : CommandHandlerBase<TransferAmountCommand, TransferAmountCommand.Result>
    {
        IEventPublisher _eventPublisher;
        ICommandSender _commandDispatcher;
        public TransferAlwaysSuccessHandler( IEventPublisher eventPublisher, ICommandSender commandDispatcher )
        {
            _eventPublisher = eventPublisher;
            _commandDispatcher = commandDispatcher;
        }
        public override async Task<TransferAmountCommand.Result> HandleAsync( TransferAmountCommand command, ICommandContext context )
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

                await _eventPublisher.PublishAsync( new AmountTransferredEvent
                {
                    AccountId = command.DestinationAccountId,
                    Amount = command.Amount
                }, context );

                await _eventPublisher.PublishAsync( new AmountTransferredEvent
                {
                    AccountId = command.DestinationAccountId,
                    Amount = command.Amount
                }, context );

                await _commandDispatcher.SendAsync( new PerformTransferAmountCommand
                {
                    Amount = command.Amount,
                    DestinationAccountId = command.DestinationAccountId,
                    SourceAccountId = command.SourceAccountId
                }, context );

                return result;
            }
        }
    }

    public class WithDrawyMoneyHandler : CommandHandlerBase<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>
    {
        public override Task<WithdrawMoneyCommand.Result> HandleAsync( WithdrawMoneyCommand command, ICommandContext commandContext )
        {
            var result = new WithdrawMoneyCommand.Result
            {
                Success = true
            };
            return Task.FromResult( result );
        }
    }

}
