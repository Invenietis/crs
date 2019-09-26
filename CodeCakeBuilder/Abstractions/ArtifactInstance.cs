using CSemVer;
using System;

namespace CodeCake.Abstractions
{
    /// <summary>
    /// Defines the instance of an <see cref="Artifact"/>: its <see cref="Version"/> is known.
    /// </summary>
    public readonly struct ArtifactInstance : IEquatable<ArtifactInstance>
    {
        /// <summary>
        /// Initializes a new <see cref="ArtifactInstance"/>.
        /// </summary>
        /// <param name="a">The artifact.</param>
        /// <param name="version">The version. Can not be null.</param>
        public ArtifactInstance( Artifact a, SVersion version )
        {
            Artifact = a;
            Version = version ?? throw new ArgumentNullException( nameof( version ) );
        }

        /// <summary>
        /// Initializes a new <see cref="ArtifactInstance"/>.
        /// </summary>
        /// <param name="type">The artifact type. Can not be null.</param>
        /// <param name="name">The artifact name. Can not be null.</param>
        /// <param name="version">The version. Can not be null.</param>
        public ArtifactInstance( string type, string name, SVersion version )
            : this( new Artifact( type, name ), version )
        {
        }

        /// <summary>
        /// Gets the artifact.
        /// </summary>
        public Artifact Artifact { get; }

        /// <summary>
        /// Gets the artifact version.
        /// </summary>
        public SVersion Version { get; }

        /// <summary>
        /// Checks equality.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>True when equals, false otherwise.</returns>
        public bool Equals( ArtifactInstance other ) => Artifact == other.Artifact && Version == other.Version;

        public override bool Equals( object obj ) => obj is ArtifactInstance a ? Equals( a ) : false;

        public override int GetHashCode() => Version.GetHashCode() ^ Artifact.GetHashCode();

        public override string ToString() => $"{Artifact}/{Version.ToNormalizedString()}";

        /// <summary>
        /// Implements == operator.
        /// </summary>
        /// <param name="x">First artifact instance.</param>
        /// <param name="y">Second artifact instance.</param>
        /// <returns>True if they are equal.</returns>
        public static bool operator ==( ArtifactInstance x, ArtifactInstance y ) => x.Equals( y );

        /// <summary>
        /// Implements != operator.
        /// </summary>
        /// <param name="x">First artifact instance.</param>
        /// <param name="y">Second artifact instance.</param>
        /// <returns>True if they are not equal.</returns>
        public static bool operator !=( ArtifactInstance x, ArtifactInstance y ) => !x.Equals( y );

    }
}
