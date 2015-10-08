using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
    public class CommandRouteAttribute : Attribute
    {
        public CommandRouteAttribute( string routePath )
        {
        }
    }
}
