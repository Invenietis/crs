﻿using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CK.Crs.Infrastructure
{
    class CrsFeature : IApplicationFeatureProvider<ControllerFeature>
    {
        ICrsModel _model;
        public CrsFeature( ICrsModel model )
        {
            _model = model;
        }
        public void PopulateFeature( IEnumerable<ApplicationPart> parts, ControllerFeature feature )
        {
            foreach( var endpoint in _model.Endpoints )
            {
                foreach( var command in endpoint.Commands )
                {
                    var crsEndpointControllerType = endpoint.EndpointType.MakeGenericType( command.CommandType ).GetTypeInfo();
                    feature.Controllers.Add( crsEndpointControllerType );
                }

                var metaControllerType = endpoint.EndpointType.MakeGenericType( typeof( MetaCommand ) ).GetTypeInfo();
                feature.Controllers.Add( metaControllerType );
            }
        }

    }
}
