using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Handlers
{

    public class UploadVideoCommand
    {
        public string Name { get; set; }

        public BlobRef Videos { get; set; }
    }

    public class UserCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public class Result
        {
            public bool Success { get; set; }
        }
    }

    public class TransferAmountCommand
    {
        public Guid SourceAccountId { get; set; }

        public Guid DestinationAccountId { get; set; }

        public decimal Amount { get; set; }

        public class Result
        {
            public DateTime EffectiveDate { get; set; }

            public DateTime CancellableDate { get; set; }
        }
    }

    public class WithdrawMoneyCommand
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

        public class Result
        {
            public bool Success { get; set; }
        }
    }
}
