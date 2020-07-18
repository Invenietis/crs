using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CK.Crs.Tests
{

    public class UploadVideoCommand
    {
        public string Name { get; set; }
    }

    public abstract class CommandBase
    {
        public int ActorId { get; set; }
    }

    public class TransferAmountCommand : CommandBase, ICommand<TransferAmountCommand.Result>
    {
        public Guid SourceAccountId { get; set; }

        public Guid DestinationAccountId { get; set; }

        public decimal Amount { get; set; }

        public class Result
        {
            public DateTime EffectiveDate { get; set; }

            public DateTime CancellableDate { get; set; }

            public Guid OperationId { get; set; }
        }
    }

    public class PerformTransferAmountCommand
    {
        public Guid SourceAccountId { get; set; }

        public Guid DestinationAccountId { get; set; }

        public decimal Amount { get; set; }
    }

    public class WithdrawMoneyCommand : ICommand<WithdrawMoneyCommand.Result>
    {
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public bool ShouldThrow { get; set; }

        public class Result
        {
            public bool Success { get; set; }
        }
    }
}
