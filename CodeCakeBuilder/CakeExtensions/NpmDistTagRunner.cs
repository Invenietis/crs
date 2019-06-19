using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.Tooling;
using System;

namespace Cake.Npm
{
    public class NpmDistTagRunner : NpmTool<NpmDistTagSettings>
    {
        public NpmDistTagRunner( IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools, ICakeLog log )
            : base( fileSystem, environment, processRunner, tools, log )
        {
        }

        public void RunDistTags( NpmDistTagSettings settings )
        {
            RunCore( settings );
        }
    }

    public enum NpmDistTagCommand
    {
        Add,
        Remove,
        List
    }

    public class NpmDistTagSettings : NpmSettings
    {
        public NpmDistTagSettings() : base( "dist-tag" )
        {

        }
        public NpmDistTagCommand DistTagCommand { get; set; }
        public string Package { get; set; }
        public string Version { get; set; }
        public string Tag { get; set; }

        /// <summary>
        /// Evaluates the settings and writes them to <paramref name="args" />.
        /// </summary>
        /// <param name="args">The argument builder into which the settings should be written.</param>
        protected override void EvaluateCore( ProcessArgumentBuilder args )
        {
            base.EvaluateCore( args );

            switch( DistTagCommand )
            {
                case NpmDistTagCommand.Add:
                    args.Append( "add" );
                    args.Append( $"{Package}@{Version}" );
                    args.Append( Tag );
                    break;
                case NpmDistTagCommand.Remove:
                    args.Append( "rm" );
                    args.Append( Package );
                    args.Append( Tag );
                    break;
                case NpmDistTagCommand.List:
                    args.Append( "ls" );
                    args.Append( Package );
                    break;
            }
        }

    }


    [CakeAliasCategory( "Npm" )]
    [CakeNamespaceImport( "Cake.Npm" )]
    public static class NpmDistTagsAliases
    {
        [CakeMethodAlias]
        [CakeAliasCategory( "DistTags" )]
        public static void NpmDistTag( this ICakeContext context, NpmDistTagSettings settings )
        {
            if( context == null ) throw new ArgumentNullException( nameof( context ) );
            if( settings == null ) throw new ArgumentNullException( nameof( settings ) );

            new NpmDistTagRunner( context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, context.Log ).RunDistTags( settings );
        }

        [CakeMethodAlias]
        [CakeAliasCategory( "DistTags" )]
        public static void NpmDistTagAdd( this ICakeContext context, string package, string version, string tag, Action<NpmDistTagSettings> configurator = null )
        {
            if( context == null ) throw new ArgumentNullException( nameof( context ) );
            if( string.IsNullOrEmpty( package ) ) throw new ArgumentNullException( nameof( package ) );
            if( string.IsNullOrEmpty( version ) ) throw new ArgumentNullException( nameof( version ) );
            if( string.IsNullOrEmpty( tag ) ) throw new ArgumentNullException( nameof( tag ) );

            NpmDistTagSettings s = new NpmDistTagSettings()
            {
                Package = package,
                Version = version,
                Tag = tag,
            };
            configurator?.Invoke( s );

            new NpmDistTagRunner( context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, context.Log ).RunDistTags( s );
        }
    }
}
