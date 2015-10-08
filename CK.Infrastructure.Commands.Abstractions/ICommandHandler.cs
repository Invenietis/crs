using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    
    public interface ICommandHandler<in T, TResult> where T : ICommand<TResult>
    {
        Task<TResult> HandleAsync( T command );
    }
}
