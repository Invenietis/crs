//using CK.Crs.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CK.Crs.Runtime.Hosting
//{
//    class EndpointBuilder
//    {
//        public IEndpointModel Build( IServiceProvider applicationServices )
//        {
//            ICrsModel model = applicationServices.GetRequiredService<ICrsModel>();
//            ICommandRegistry registry = applicationServices.GetRequiredService<ICommandRegistry>();

//            var config = new CrsEndpointConfiguration( registry, model );

//            var settings = applicationServices.GetService<JsonSerializerSettings>() ?? new JsonSerializerSettings
//            {
//                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
//            };
//            config.ChangeDefaultBinder( new JsonCommandBinder( settings ) );
//            config.ChangeDefaultFormatter( new JsonResponseFormatter( settings ) );

//            // Override configuration from the given lambda
//            configuration?.Invoke( config );

//            IEndpointModel endpointModel = config.Build( crsPath.Value );
//        }
//    }
//}
