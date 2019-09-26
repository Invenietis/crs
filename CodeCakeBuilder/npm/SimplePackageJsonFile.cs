using CK.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeCake
{
    public readonly struct SimplePackageJsonFile
    {
        public readonly NormalizedPath JsonFilePath;
        public readonly string Name;
        public readonly string Scope;
        public readonly string ShortName;
        public readonly string Version;
        public readonly IReadOnlyList<string> Scripts;
        public readonly bool IsPrivate;

        SimplePackageJsonFile(
            NormalizedPath jsonFilePath,
            string name,
            string scope,
            string shortName,
            string version,
            IReadOnlyList<string> scripts,
            bool isPrivate )
        {
            JsonFilePath = jsonFilePath;
            Name = name;
            Scope = scope;
            ShortName = shortName;
            Version = version;
            Scripts = scripts;
            IsPrivate = isPrivate;
        }

        public static SimplePackageJsonFile Create( NormalizedPath folderPath )
        {
            var jsonFilePath = folderPath.AppendPart( "package.json" );
            JObject json = JObject.Parse( File.ReadAllText( jsonFilePath ) );
            string name = json.Value<string>( "name" );
            bool isPrivate = json.Value<bool?>( "private" ) ?? false;
            string shortName;
            string scope;
            Match m;
            if( name != null
                && (m = Regex.Match( name, "(?<1>@[a-z\\d][\\w-.]+)/(?<2>[a-z\\d][\\w-.]*)", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture )).Success )
            {
                scope = m.Groups[1].Value;
                shortName = m.Groups[2].Value;
            }
            else
            {
                scope = null;
                shortName = name;
            }
            string version = json.Value<string>( "version" );
            IReadOnlyList<string> scripts;
            if( json.TryGetValue( "scripts", out JToken scriptsToken ) && scriptsToken.HasValues )
            {
                scripts = scriptsToken.Children<JProperty>().Select( p => p.Name ).ToArray();
            }
            else
            {
                scripts = Array.Empty<string>();
            }
            return new SimplePackageJsonFile( jsonFilePath, name, scope, shortName, version, scripts, isPrivate );
        }
    }
}
