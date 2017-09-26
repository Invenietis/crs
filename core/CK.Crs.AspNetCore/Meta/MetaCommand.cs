using CK.Crs.Infrastructure;
using System.Collections.Generic;

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

            public class MetaCommandDescription
            {
                public string CommandType { get; set; }
                public string CommandName { get; set; }
                public string ResultType { get; set; }
                public string Traits { get; set; }
                public string Description { get; set; }
                public RequestPropertyInfo[] Parameters { get; set; }
            }
        }
    }
}
