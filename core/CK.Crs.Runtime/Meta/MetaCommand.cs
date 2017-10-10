using CK.Crs.Infrastructure;
using System.Collections.Generic;
using CK.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace CK.Crs
{

    public class MetaCommand : ICommand<MetaCommand.Result>
    {
        public bool ShowCommands { get; set; }
        public bool ShowAmbientValues { get; set; }
        public class Result
        {
            public int Version { get; set; }
            public IDictionary<string, object> AmbientValues { get; set; }
            public Dictionary<string, MetaCommandDescription> Commands { get; set; }

            public string CallerIdPropertyName { get; set; }

            public class MetaCommandDescription
            {
                public string CommandType { get; set; }
                public string CommandName { get; set; }
                public string ResultType { get; set; }
                public string Traits { get; set; }
                public string Description { get; set; }
                public RequestPropertyInfo[] Parameters { get; set; }
            }

            public static async Task<MetaCommandResponse> CreateAsync( MetaCommand command, IEndpointModel endpointModel, IAmbientValues ambientValues, IAmbientValuesRegistration registration )
            {
                var result = new Result
                {
                    Version = 1,
                    CallerIdPropertyName = endpointModel.CallerIdName
                };

                if( command.ShowAmbientValues )
                {
                    result.AmbientValues = new Dictionary<string, object>();
                    foreach( var a in registration.AmbientValues )
                    {
                        if( ambientValues.IsDefined( a.Name ) )
                        {
                            var o = await ambientValues.GetValueAsync( a.Name );
                            result.AmbientValues.Add( a.Name, o );
                        }
                    }
                }
                if( command.ShowCommands )
                {
                    result.Commands = new Dictionary<string, MetaCommandDescription>();
                    foreach( var c in endpointModel.Commands )
                    {
                        MetaCommandDescription desc = new MetaCommandDescription
                        {
                            CommandName = c.Name,
                            CommandType = c.CommandType.FullName,
                            ResultType = c.ResultType?.FullName,
                            Parameters = c.CommandType.GetTypeInfo().DeclaredProperties.Select( e => new RequestPropertyInfo( e, registration ) ).ToArray(),
                            Traits = c.Tags?.ToString(),
                            Description = c.Description
                        };
                        result.Commands.Add( desc.CommandName, desc );
                    }
                }
                return new MetaCommandResponse( result );
            }
        }
    }
}
