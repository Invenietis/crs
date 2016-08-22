using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    public class CrsExecutorConfiguration
    {
        /// <summary>
        /// Gets or sets the store that manage persistence of running (live) commands. Defaults to <see cref="InMemoryCommandRunningStore"/>.
        /// </summary>
        public Type CommandRunningStore { get; set; } = typeof( InMemoryCommandRunningStore );
    }

}
