using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs.Samples.AspNetCoreApp.Core
{
    public class CrsControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        ICommandRegistry _commands;
        public CrsControllerFeatureProvider(ICommandRegistry commands)
        {
            _commands = commands;
        }
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var command in _commands.Registration)
            {
                // TODO: populate all discovered Crs Controllers and associates endpoint with commands.

                var controllerType2 = typeof(MyCrsSlimController<>).MakeGenericType(command.CommandType).GetTypeInfo();
                feature.Controllers.Add(controllerType2);
            }
        }
    }
}
