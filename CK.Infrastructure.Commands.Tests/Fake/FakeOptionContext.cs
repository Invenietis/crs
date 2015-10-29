using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;

namespace CK.Infrastructure.Commands.Tests
{
    class OptionStub : IOptions<CommandReceiverOptions>
    {
        public OptionStub()
        {
            Value = new CommandReceiverOptions();
        }
        public CommandReceiverOptions Value { get; set; }
    }
}
