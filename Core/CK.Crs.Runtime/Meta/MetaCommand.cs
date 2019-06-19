using System.Collections.Generic;
using CK.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace CK.Crs.Meta
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

            public static MetaCommandResponse Create( MetaCommand command, IEndpointModel endpointModel )
            {
                var result = new Result
                {
                    Version = 1,
                    CallerIdPropertyName = endpointModel.CallerIdName
                };
                if( command.ShowCommands )
                {
                    result.Commands = new Dictionary<string, MetaCommandDescription>();
                    foreach( var c in endpointModel.Commands )
                    {
                        MetaCommandDescription desc = new MetaCommandDescription
                        {
                            CommandName = c.Name.ToString(),
                            CommandType = c.CommandType.FullName,
                            ResultType = c.ResultType?.FullName,
                            Parameters = c.CommandType.GetTypeInfo().DeclaredProperties.Select( e => new RequestPropertyInfo( e, null ) ).ToArray(),
                            Traits = c.Tags?.ToString(),
                            Description = c.Description
                        };
                        result.Commands.Add( desc.CommandName, desc );
                    }
                }
                return new MetaCommandResponse( result );
            }
            public static async Task<MetaCommandResponse> CreateAsync( MetaCommand command, IEndpointModel endpointModel, IServiceProvider services )
            {
                var result = new Result
                {
                    Version = 1,
                    CallerIdPropertyName = endpointModel.CallerIdName
                };
                var registration = services.GetService<IAmbientValuesRegistration>();
                if( command.ShowAmbientValues )
                {
                    if( registration != null )
                    {
                        var ambientValues = services.GetService<IAmbientValues>();
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
