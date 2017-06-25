using System;
using CK.Crs.Runtime;
using System.Xml.Linq;

namespace CK.Crs.Scalability.FileSystem
{
    public class FileSystemConfiguration
    {
        public FileSystemConfiguration(string path)
        {
            Path = path;
        }

        public string Path { get; set; }

        public string Extension { get; set; } = ".command";

        public string FormatResponsePath( CommandResponse commandResponse)
        {
            return System.IO.Path.Combine( Path, commandResponse.CommandId.ToString("N") + Extension);
        }
        
    }
}