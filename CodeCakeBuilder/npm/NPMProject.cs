using Cake.Common.Diagnostics;
using Cake.Npm;
using Cake.Npm.RunScript;
using CK.Text;
using CSemVer;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using SimpleGitVersion;

namespace CodeCake
{
    /// <summary>
    /// Basic (unpublished) NPM project.
    /// </summary>
    public class NPMProject
    {

        protected NPMProject(
            StandardGlobalInfo globalInfo,
            NPMSolution npmSolution,
            SimplePackageJsonFile json,
            NormalizedPath outputPath )
        {
            DirectoryPath = json.JsonFilePath.RemoveLastPart();
            PackageJson = json;
            OutputPath = outputPath;
            NPMRCPath = DirectoryPath.AppendPart( ".npmrc" );
            GlobalInfo = globalInfo;
            NpmSolution = npmSolution;
        }

        protected static NPMProject CreateNPMProject(
            StandardGlobalInfo globalInfo,
            NPMSolution npmSolution,
            SimplePackageJsonFile json,
            NormalizedPath outputPath )
        {
            return new NPMProject( globalInfo, npmSolution, json, outputPath );
        }


        public StandardGlobalInfo GlobalInfo { get; }

        public NPMSolution NpmSolution { get; }

        public virtual bool IsPublished => false;

        public NormalizedPath DirectoryPath { get; }

        public SimplePackageJsonFile PackageJson { get; }

        public NormalizedPath OutputPath { get; }

        public NormalizedPath NPMRCPath { get; }

        /// <summary>
        /// Runs "npm ci" (instead of "npm install") on this project.
        /// See https://docs.npmjs.com/cli/ci.html.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        public virtual void RunNpmCi()
        {
            GlobalInfo.Cake.Information( $"Running 'npm ci' in {DirectoryPath.Path}" );
            GlobalInfo.Cake.NpmCi( settings =>
            {
                settings.LogLevel = NpmLogLevel.Warn;
                settings.WorkingDirectory = DirectoryPath.Path;
            } );
        }

        /// <summary>
        /// Gets whether this package has a named script entry.
        /// </summary>
        /// <param name="name">Script name.</param>
        /// <returns>Whether the script exists.</returns>
        public bool HasScript( string name ) => PackageJson.Scripts.Contains( name );

        /// <summary>
        /// Finds either "baseName-debug", "baseName-release" depending on <see cref="ICommitBuildInfo.BuildConfiguration"/>
        /// and falls back to baseName if specific scripts don't exist.
        /// By default, at least 'baseName' must exist otherwise an InvalidOperationException is thrown.
        /// </summary>
        /// <param name="baseName">Base name to look for.</param>
        /// <param name="checkBaseNameExist">
        /// By default, at least baseName must exist otherwise an InvalidOperationException is thrown.
        /// When false, no check is done for baseName that is returned as-is.
        /// When null (and baseName cannot be found), null is returned.
        /// </param>
        /// <returns>The best script (or null if it doesn't exist and <paramref name="checkBaseNameExist"/> is null).</returns>
        public string FindBestScript( string baseName, bool? checkBaseNameExist = true )
        {
            string n = baseName + '-' + GlobalInfo.BuildInfo.BuildConfiguration;
            if( HasScript( n ) ) return n;
            if( checkBaseNameExist == null )
            {
                return HasScript( baseName ) ? baseName : null;
            }
            else if( checkBaseNameExist == true )
            {
                if( HasScript( baseName ) ) return baseName;
                throw new InvalidOperationException( $"Missing script '{baseName}' in {PackageJson.JsonFilePath}." );
            }
            return baseName;
        }

        /// <summary>
        /// Finds either "name-debug", "name-release" depending on <see cref="StandardGlobalInfo.IsRelease"/>
        /// and falls back to "name". By default if no script is found an <see cref="InvalidOperationException"/>
        /// is thrown. To emit a Cake warning (and return null) if the script can't be found,
        /// let <paramref name="scriptMustExist"/> be false.
        /// </summary>
        /// <param name="globalInfo">The global info object.</param>
        /// <param name="name">Base script name to look for.</param>
        /// <param name="scriptMustExist">
        /// False to emit a warning and return null if the script doesn't exist.
        /// By default if no script is found an <see cref="InvalidOperationException"/> is thrown.
        /// </param>
        /// <returns>The best script (or null if it doesn't exist and <paramref name="scriptMustExist"/> is false).</returns>
        public string FindBestScript( string name, bool scriptMustExist = true )
        {
            string n = FindBestScript( name, scriptMustExist ? (bool?)true : null );
            if( n == null )
            {
                GlobalInfo.Cake.Warning( $"Missing script '{name}' in '{PackageJson.JsonFilePath}'." );
            }
            return n;
        }

        /// <summary>
        /// Run clean script (that must exist, see <see cref="FindBestScript(string, bool)"/>). 
        /// </summary>
        /// <param name="cleanScriptName">Clean script name.</param>
        /// <returns></returns>
        public virtual void RunClean()
        {
            RunScript( "clean", false, true );
        }

        /// <summary>
        /// Runs the 'name-debug', 'name-release' or 'name' script (see <see cref="FindBestScript(string, bool)"/>).
        /// </summary>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        /// <param name="runInBuildDirectory">Whether the script should be run in <see cref="OutputPath"/> or <see cref="DirectoryPath"/> if false.</param>
        /// <returns>False if the script doesn't exist (<paramref name="scriptMustExist"/> is false), otherwise true.</returns>
        public bool RunScript( string name, bool runInBuildDirectory, bool scriptMustExist )
        {
            string n = FindBestScript( name, scriptMustExist );
            if( n == null ) return false;
            DoRunScript( n, runInBuildDirectory );
            return true;
        }

        /// <summary>
        /// Run a npm script.
        /// </summary>
        /// <param name="scriptName">The npm script to run.</param>
        /// <param name="runInBuildDirectory">Whether the script should be run in <see cref="OutputPath"/> or <see cref="DirectoryPath"/> if false.</param>
        private protected virtual void DoRunScript( string scriptName, bool runInBuildDirectory )
        {
            GlobalInfo.Cake.NpmRunScript(
                    new NpmRunScriptSettings()
                    {
                        ScriptName = scriptName,
                        LogLevel = NpmLogLevel.Info,
                        WorkingDirectory = runInBuildDirectory ? OutputPath.Path : DirectoryPath.Path
                    }
                );
        }

        /// <summary>
        /// Runs "build" script: see <see cref="RunScript(string, bool)"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        /// <returns>False if the script doesn't exist (<paramref name="scriptMustExist"/> is false), otherwise true.</returns>
        public bool RunBuild( bool scriptMustExist = true ) => RunScript( "build", false, scriptMustExist );

        /// <summary>
        /// Runs "test" script: see <see cref="RunScript(string, bool)"/>.
        /// </summary>
        /// <param name="globalInfo">The global information object.</param>
        /// <param name="scriptMustExist">
        /// False to only emit a warning and return false if the script doesn't exist instead of
        /// throwing an exception.
        /// </param>
        /// <returns>False if the script doesn't exist (<paramref name="scriptMustExist"/> is false), otherwise true.</returns>
        public void RunTest()
        {
            var key = DirectoryPath.AppendPart( "test" );
            if( !GlobalInfo.CheckCommitMemoryKey( key ) )
            {
                RunScript( "test", false, true );
                GlobalInfo.WriteCommitMemoryKey( key );
            }
        }

        private protected IDisposable TemporarySetPackageVersion( SVersion version, bool targetOutputPath = false )
        {
            return TempFileTextModification.TemporaryReplacePackageVersion( !targetOutputPath ? PackageJson.JsonFilePath : OutputPath.AppendPart( "package.json" ), version );
        }

        private protected IDisposable TemporaryPrePack( SVersion version, Action<JObject> packageJsonPreProcessor, bool ckliLocalFeedMode )
        {
            return TempFileTextModification.TemporaryReplaceDependenciesVersion( NpmSolution, OutputPath.AppendPart( "package.json" ), ckliLocalFeedMode, version, packageJsonPreProcessor );
        }

        #region .npmrc configuration


        public IDisposable TemporarySetPushTargetAndTokenLogin( string pushUri, string token )
        {
            return TempFileTextModification.TemporaryInjectNPMToken( NPMRCPath, pushUri, PackageJson.Scope, ( npmrc, u ) => npmrc.Add( u + ":_authToken=" + token ) );
        }

        public IDisposable TemporarySetPushTargetAndPasswordLogin( string pushUri, string password )
        {
            return TempFileTextModification.TemporaryInjectNPMToken( NPMRCPath, pushUri, PackageJson.Scope, ( npmrc, u ) =>
            {
                npmrc.Add( u + ":username=CodeCakeBuilder" );
                npmrc.Add( u + ":_password=" + password );
            } );
        }

        public IDisposable TemporarySetPushTargetAndAzurePatLogin( string pushUri, string pat )
        {
            var pwd = Convert.ToBase64String( Encoding.UTF8.GetBytes( pat ) );
            return TemporarySetPushTargetAndPasswordLogin( pushUri, pwd );
        }
        #endregion
    }
}
