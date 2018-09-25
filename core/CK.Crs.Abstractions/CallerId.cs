using System;
using System.Globalization;
using System.Linq;

namespace CK.Crs
{
    public struct CallerId
    {
        /// <summary>
        /// A readonly instance of the structure will a null procotol and no values.
        /// </summary>
        public static readonly CallerId None = new CallerId( String.Empty, CK.Core.Util.Array.Empty<string>() );

        private const char Separator = '|';

        private string _token;
        private string[] _values;

        public CallerId( string protocol, string[] values )
        {
            Protocol = protocol ?? throw new ArgumentNullException( nameof( protocol ) );
            _values = values;

            _token = string.Concat(
                protocol,
                Separator,
                string.Join(
                    Separator.ToString( CultureInfo.InvariantCulture ),
                    _values.Select( Uri.EscapeDataString )
                    )
                );
        }

        public bool IsValid => !String.IsNullOrEmpty( Protocol );

        public string Protocol { get; }

        public string[] Values
        {
            get
            {
                if( _values == null ) return Array.Empty<string>();
                return _values;
            }
        }

        /// <summary>
        /// Gets the raw value of this token.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _token;

        public static CallerId Parse( string token )
        {
            if( String.IsNullOrEmpty( token ) ) return CallerId.None;
            var s = token.Split( new char[] { Separator }, StringSplitOptions.None );
            if( s.Length > 1 )
            {
                var parameters = new string[s.Length - 1];
                Array.ConstrainedCopy( s, 1, parameters, 0, s.Length - 1 );
                return new CallerId( s[0], parameters.Select( Uri.UnescapeDataString ).ToArray() );
            }
            return CallerId.None;
        }

        public override bool Equals( object obj )
        {
            CallerId other = (CallerId)obj;
            bool equals = other.Protocol == Protocol && other.Values.Length == Values.Length;
            if( equals )
            {
                for( int i = 0; i < Values.Length; ++i ) equals &= other.Values[i] == Values[i];
            }
            return equals;
        }

        public override int GetHashCode()
        {
            if( String.IsNullOrEmpty( Protocol ) ) return 0;
            if( Values.Length == 0 ) return 0;

            int s = Protocol.GetHashCode();
            for( int i = 0; i < Values.Length; ++i ) s ^= Values[i].GetHashCode();
            return s;
        }

    }
}
