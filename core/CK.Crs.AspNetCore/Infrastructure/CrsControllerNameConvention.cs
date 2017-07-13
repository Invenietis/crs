using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs.Infrastructure
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    class CrsControllerNameConvention : Attribute, IControllerModelConvention
    {
        private ICrsModel _model;

        public CrsControllerNameConvention( ICrsModel model )
        {
            _model = model;
        }

        public void Apply( ControllerModel controller )
        {
            if( controller.ControllerType.IsGenericType &&
                ReflectionUtil.IsAssignableToGenericType(
                    controller.ControllerType.GetGenericTypeDefinition(),
                    typeof( ICrsReceiver<> ) ) )
            {
                ICrsReceiverModel endpointModel = _model.GetReceiver( controller.ControllerType.AsType() );
                if( endpointModel != null )
                {
                    controller.ControllerName = endpointModel.Name;
                }
            }
        }
    }

}
