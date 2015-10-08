using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommand
    {
        Guid CommandId { get; set; }
    }
    public interface ICommand<TResult> : ICommand
    {
    }

    public class CommandBase : ICommand
    {
        public Guid CommandId { get; set; }
    }

    public class CommandBase<TResult> : CommandBase, ICommand<TResult>
    {
    }
}
