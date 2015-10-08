using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandResultSerializer
    {
        Task SerializeCommandResult( ICommandResult result, Stream outputStream );
    }
}
