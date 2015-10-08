using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandReceiverOptions
    {
        /// <summary>
        /// Gets or sets the route prefix of the command route path.
        /// </summary>
        string RoutePrefix { get; }

        CommandRouteOptions CommandRouteOptions { get; }
    }

    public class CommandRouteOptions
    {
        
        public bool AddConventionRoutesToMap { get; set; }
    }

}
