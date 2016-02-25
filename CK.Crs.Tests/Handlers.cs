﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Tests.Handlers
{
    public class TransferAlwaysSuccessHandler : CommandHandler<TransferAmountCommand, TransferAmountCommand.Result>
    {
        protected override Task<TransferAmountCommand.Result> DoHandleAsync( ICommandExecutionContext commandContext, TransferAmountCommand command )
        {
            using( commandContext.Monitor.OpenInfo().Send( $"Transferring {command.Amount} from {command.SourceAccountId} to {command.DestinationAccountId} " ) )
            {
                var result = new TransferAmountCommand.Result
                {
                    EffectiveDate = DateTime.UtcNow.Date.AddDays( 2 ),
                    CancellableDate = DateTime.UtcNow.AddHours( 1 )
                };
                commandContext.Monitor.Info().Send( $"Transfer will be effective at {result.EffectiveDate.ToString()}." );
                commandContext.Monitor.Info().Send( $"You have one hour to cancel it." );
                return Task.FromResult( result );
            }
        }
    }


    [Transaction]
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
