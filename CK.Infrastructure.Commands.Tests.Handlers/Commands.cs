﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands.Tests.Handlers
{

    [Framework.AsyncCommand]
    public class UploadVideoCommand
    {
        public string Name { get; set; }

        public Framework.BlobRef Videos { get; set; }
    }

    [Framework.AsyncCommand]
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
