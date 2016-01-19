﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands.Tests.Handlers
{
    public class TransferAlwaysSuccessHandler : CommandHandler<TransferAmountCommand, TransferAmountCommand.Result>
    {
        protected override Task<TransferAmountCommand.Result> DoHandleAsync( CommandContext<TransferAmountCommand> command )
        {
            using( command.Monitor.OpenInfo().Send( $"Transferring {command.Command.Amount} from {command.Command.SourceAccountId} to {command.Command.DestinationAccountId} " ) )
            {
                var result = new TransferAmountCommand.Result
                {
                    EffectiveDate = DateTime.UtcNow.Date.AddDays( 2 ),
                    CancellableDate = DateTime.UtcNow.AddHours( 1 )
                };
                command.Monitor.Info().Send( $"Transfer will be effective at {result.EffectiveDate.ToString()}." );
                command.Monitor.Info().Send( $"You have one hour to cancel it." );
                return Task.FromResult( result );
            }
        }
    }


    public class TransferAlwaysSuccessHandlerWithDecoration : TransferAlwaysSuccessHandler
    {
        [Transaction]
        protected override Task<TransferAmountCommand.Result> DoHandleAsync( CommandContext<TransferAmountCommand> command )
        {
            return base.DoHandleAsync( command );
        }
    }

    public class WithDrawyMoneyHandler : CommandHandler<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>
    {
        protected override Task<WithdrawMoneyCommand.Result> DoHandleAsync( CommandContext<WithdrawMoneyCommand> command )
        {
            var result =  new WithdrawMoneyCommand.Result
            {
                Success = true
            };
            return Task.FromResult( result );
        }
    }

}
