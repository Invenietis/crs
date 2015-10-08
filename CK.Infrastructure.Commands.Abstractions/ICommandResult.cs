using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandResult
    {
        Guid CommandId { get; }
        bool IsSuccess { get; }
        bool IsFailure { get; }
        object Result { get; set; }
    }
}
