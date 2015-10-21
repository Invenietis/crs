using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRequest
    {
        object Command { get; }

        string CallbackId { get; }

        Type CommandServerType { get; }
    }

}
