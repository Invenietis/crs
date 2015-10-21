using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands.Tests.Handlers
{
    public class TransferAlwaysSuccessHandler : Framework.CommandHandler<TransferAmountCommand, TransferAmountCommand.Result>
    {
        public override Task<TransferAmountCommand.Result> DoHandleAsync( TransferAmountCommand command )
        {
            var result = new TransferAmountCommand.Result
            {
                EffectiveDate = DateTime.UtcNow.Date.AddDays( 2 ),
                CancellableDate = DateTime.UtcNow.AddHours( 1 )
            };
            return Task.FromResult( result );
        }
    }

    public class WithDrawyMoneyHandler : Framework.CommandHandler<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>
    {
        public override Task<WithdrawMoneyCommand.Result> DoHandleAsync( WithdrawMoneyCommand command )
        {
            var result =  new WithdrawMoneyCommand.Result
            {
                Success = true
            };
            return Task.FromResult( result );
        }
    }

}
