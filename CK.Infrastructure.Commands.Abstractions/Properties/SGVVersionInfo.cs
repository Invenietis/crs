
// Automatically generated by 'sgv prebuild' Working folder has non committed changes..
using System;
using System.Reflection;

[assembly: AssemblyVersion(@"0.0")]
[assembly: AssemblyFileVersion(@"0.0.0.0")]
[assembly: AssemblyInformationalVersion(@"Working folder has non committed changes. Sha:8af9468d728be0984c35223b910041d82bade785 User:CEDRIC-LAPTOP\cedri_000")]

[assembly: SimpleGitVersionInfo( "Working folder has non committed changes.", "2016-01-13 09:47:36Z", "0.12.1" )]

/// <summary>
/// Automatically generated by 'sgv prebuild' Working folder has non committed changes.. 
/// </summary>
class SimpleGitVersionInfoAttribute : Attribute
{
    public SimpleGitVersionInfoAttribute( string semVer, string buildTimeUtc, string sgvVersion )
    {
        SemVer = semVer;
        BuildTimeUtc = buildTimeUtc;
        SGVVersion = sgvVersion;
    }

    public readonly string SemVer;
    public readonly string BuildTimeUtc;
    public readonly string SGVVersion;
    
    public override string ToString()
    {
        return String.Format( "SemVer: {0}, BuildTimeUtc: {1}, SGVVersion: {2}", SemVer, BuildTimeUtc, SGVVersion );
    }
}
