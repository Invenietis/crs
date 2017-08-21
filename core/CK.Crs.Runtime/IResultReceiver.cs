using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{

    public interface IResultReceiver
    {
        Task ReceiveResult( object result, ICommandContext context );
    }
}
