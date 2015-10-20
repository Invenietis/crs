using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class ConventionCommandTypeSelector : ICommandTypeSelector
    {

        public Type DetermineCommandType( ICommandReceiverOptions receiverOptions, CommandRoutePath routePath )
        {
            string commandClassName = routePath.ExtractCommandClassName();
            string commandTypeName = String.Format( "{0}.{1}", receiverOptions.CommandRouteOptions.DefaultCommandNamespace, commandClassName);

            AssemblyName commandAssemblyName = new AssemblyName( receiverOptions.CommandRouteOptions.DefaultCommandAssemblyName );
            Assembly commandsAssembly = Assembly.Load( commandAssemblyName );

            return commandsAssembly.GetType( commandTypeName);
        }
    }
}
