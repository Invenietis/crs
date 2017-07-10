using CK.Crs.Infrastructure;
using System.Collections.Generic;

namespace CK.Crs
{
    class MetaCommand
    {
        public bool ShowCommands { get; set; }
        public bool ShowAmbientValues { get; set; }
        public class MetaResult
        {
            public int Version { get; set; }
            public IDictionary<string, object> AmbientValues { get; set; }
            public Dictionary<string, MetaCommandDescription> Commands { get; set; }
            public class MetaCommandDescription
            {
                public string CommandType { get; internal set; }
                public string CommandName { get; internal set; }
                public string Traits { get; internal set; }
                public string Description { get; internal set; }
                public RequestPropertyInfo[] Parameters { get; internal set; }
            }
        }
    }
}
