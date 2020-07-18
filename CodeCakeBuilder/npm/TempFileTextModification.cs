using Cake.Common.Diagnostics;
using Cake.Core;
using CK.Text;
using CSemVer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeCake
{
    public class TempFileTextModification : IDisposable
    {
        readonly string _originalText;
        readonly NormalizedPath _path;
        private readonly bool _filePreviouslyExisted;

        TempFileTextModification(
            string savedPackageJson,
            NormalizedPath path,
            bool filePreviouslyExisted
            )
        {
            _originalText = savedPackageJson;
            _path = path;
            _filePreviouslyExisted = filePreviouslyExisted;
        }

        protected TempFileTextModification( TempFileTextModification toCopy )
        {
            _originalText = toCopy._originalText;
            _path = toCopy._path;
        }

        public static (string content, TempFileTextModification temp) CreateTempFileTextModification( NormalizedPath path )
        {
            bool fileExist = File.Exists( path );
            if( !fileExist ) File.Create( path ).Dispose();
            string txt = File.ReadAllText( path );
            return (txt, new TempFileTextModification( txt, path, fileExist ));
        }

        /// <summary>
        /// Revert the change made to the package.json
        /// </summary>
        public void Dispose()
        {
            if( !_filePreviouslyExisted )
            {
                File.Delete( _path );
                return;
            }
            File.WriteAllText( _path, _originalText );
        }


        /// <summary>
        /// Set the package version and change the project reference to package reference.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="version"></param>
        /// <param name="preparePack"></param>
        /// <param name="packageJsonPreProcessor"></param>
        /// <returns></returns>
        public static IDisposable TemporaryReplacePackageVersion(
            NormalizedPath jsonPath,
            SVersion version)
        {
            (string content, TempFileTextModification temp) = CreateTempFileTextModification( jsonPath );
            JObject json = JObject.Parse( content );
            json["version"] = version.ToNormalizedString();
            File.WriteAllText( jsonPath, json.ToString() );
            return temp;
        }

        public static IDisposable TemporaryReplaceDependenciesVersion(
            NPMSolution npmSolution,
            NormalizedPath jsonPath,
            bool ckliLocalFeedMode,
            SVersion version,
            Action<JObject> packageJsonPreProcessor
        )
        {
            (string content, TempFileTextModification temp) = CreateTempFileTextModification( jsonPath );
            JObject json = JObject.Parse( content );
            packageJsonPreProcessor?.Invoke( json );
            json.Remove( "devDependencies" );
            foreach( var dependencyPropName in new string[]
            {
                "dependencies",
                "peerDependencies",
                "bundledDependencies",
                "optionalDependencies",
            } )
            {
                if( json.ContainsKey( dependencyPropName ) )
                {
                    JObject dependencies = (JObject)json[dependencyPropName];
                    foreach( KeyValuePair<string, JToken> keyValuePair in dependencies )
                    {
                        if( npmSolution.AllPublishedProjects.FirstOrDefault( x => x.PackageJson.Name == keyValuePair.Key )
                            is NPMPublishedProject localProject )
                        {
                            dependencies[keyValuePair.Key] = new JValue( "^" + version );
                        }
                    }
                    if( ckliLocalFeedMode )
                    {
                        foreach( var dependency in ((IEnumerable<KeyValuePair<string, JToken>>)dependencies)
                            .Select( s => new KeyValuePair<string, string>( s.Key, s.Value.ToString() ) )
                            .Where( p => p.Value.StartsWith( "file:" ) )
                            .Select( s => new KeyValuePair<string, SVersion>( s.Key, ParseVersionFromPackagePath( s.Value ) ) ) )
                        {
                            dependencies[dependency.Key] = dependency.Value.ToNormalizedString();
                        }
                    }
                }
            }
            File.WriteAllText( jsonPath, json.ToString() );
            return temp;
        }

        /// <summary>
        /// Parses a full path and extracts a <see cref="SVersion"/>.
        /// </summary>
        /// <param name="fullPath">The full path of the package.</param>
        /// <returns>The <see cref="SVersion"/> of the package.</returns>
        static SVersion ParseVersionFromPackagePath( string fullPath )
        {
            var fName = Path.GetFileNameWithoutExtension( fullPath );
            int idxV = Regex.Match( fName, "\\.\\d" ).Index;
            var id = fName.Substring( 0, idxV );
            return SVersion.Parse( fName.Substring( idxV + 1 ) );
        }

        public static TempFileTextModification TemporaryInjectNPMToken( NormalizedPath npmrcPath, string pushUri, string scope, Action<List<string>, string> configure )
        {
            (string content, TempFileTextModification temp) = CreateTempFileTextModification( npmrcPath );
            List<string> npmrc = content.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            ).ToList();
            if( string.IsNullOrEmpty( scope ) )
            {
                npmrc.Add( "registry=" + pushUri );
            }
            else
            {
                Debug.Assert( scope[0] == '@' );
                npmrc.Add( scope + ":registry=" + pushUri );
            }
            pushUri = pushUri.Replace( "https:", "" );
            npmrc.Add( pushUri + ":always-auth=true" );
            configure( npmrc, pushUri );
            File.WriteAllLines( npmrcPath, npmrc );
            return temp;
        }
    }
}
