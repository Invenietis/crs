
// Automatically generated by 'sgv prebuild' Working folder has non committed changes..
using System;
using System.Reflection;

[assembly: AssemblyVersion(@"0.0")]
[assembly: AssemblyFileVersion(@"0.0.0.0")]
[assembly: AssemblyInformationalVersion(@"Working folder has non committed changes. Sha:f436a2355532c1a58c8609670bc2c5d7e5b31c5a User:PIMOUSSE\Hippo")]

[assembly: SimpleGitVersionInfo( "Working folder has non committed changes.", "2016-01-15 13:58:19Z", "0.12.1" )]

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
