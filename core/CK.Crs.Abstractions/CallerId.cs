using System;
using System.Text;

namespace CK.Crs
{
    public struct CallerId
    {
        /// <summary>
        /// A readonly instance of the structure will a null procotol and no values.
        /// </summary>
        public static readonly CallerId None = new CallerId( null, Span<string>.Empty );

        private const char Separator = ':';
        public CallerId( string protocol, Span<string> values )
        {
            Protocol = protocol;
            Values = values.ToArray();
        }

        public string Protocol { get; }

        public string[] Values { get; }

        /// <summary>
        /// Gets the raw value of this token.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append( Protocol );
            for( int i = 0; i < Values.Length; ++i ) b.AppendFormat( "{0}{1}", Separator, Values[i] );
            return b.ToString();
        }

        public static CallerId Parse( string token )
        {
            var s = token.Split( new char[] { ':' }, StringSplitOptions.None );
            if( s.Length > 1 )
            {
                return new CallerId( s[0], s.AsSpan().Slice( 1 ) );
            }
            return default( CallerId );
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
            if( Protocol == null ) return 0;
            if( Values == null ) return 0;

            int s = Protocol.GetHashCode();
            for( int i = 0; i < Values.Length; ++i ) s ^= Values[i].GetHashCode();
            return s;
        }

    }
}
