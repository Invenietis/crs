using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Crs.Tests.Integration;

namespace CK.Crs.Handlers
{

    [Transaction]
    public class TransferAlwaysSuccessHandler : CommandHandler<TransferAmountCommand, TransferAmountCommand.Result>
    {
        protected override Task<TransferAmountCommand.Result> DoHandleAsync( Command<TransferAmountCommand> command )
        {
            var result = new TransferAmountCommand.Result
            {
                EffectiveDate = DateTime.UtcNow.Date.AddDays( 2 ),
                CancellableDate = DateTime.UtcNow.AddHours( 1 )
            };
            return Task.FromResult( result );
        }
    }

    public class WithDrawyMoneyHandler : CommandHandler<WithdrawMoneyCommand, WithdrawMoneyCommand.Result>
    {
        protected override Task<WithdrawMoneyCommand.Result> DoHandleAsync( Command<WithdrawMoneyCommand> command )
        {
            var result =  new WithdrawMoneyCommand.Result
            {
                Success = true
            };
            return Task.FromResult( result );
        }
    }

    [Transaction]
    public class UserHandler : CommandHandler<UserCommand, UserCommand.Result>
    {
        IRepository<UserModel> _repository;
        public UserHandler( IRepository<UserModel> repository )
        {
            _repository = repository;
        }
        protected override Task<UserCommand.Result> DoHandleAsync( Command<UserCommand> command )
        {
            _repository.Add( new UserModel
            {
                Id = Guid.NewGuid(),
                FirstName = command.Model.FirstName,
                LastName = command.Model.LastName
            } );
            return Task.FromResult( new UserCommand.Result { Success = true } );
        }
    }
}
