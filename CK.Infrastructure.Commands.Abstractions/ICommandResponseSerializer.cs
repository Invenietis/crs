using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandResponseSerializer
    {
        void Serialize( ICommandResponse response, Stream outputStream );
    }
}
