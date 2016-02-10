using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CK.Authentication;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public class IdentityChecker : IIdentityChecker
    {
        IEnumerable<IValueExtractor> _defaultExtractors;
        public IdentityChecker()
        {
            _defaultExtractors = new IValueExtractor[]
            {
                new ClaimValueExtractor( IdentityClaimTypes.ActorId, "ActorId" ),
                new ClaimValueExtractor( IdentityClaimTypes.AnonymousId, "AnonymousId" ),
                new ClaimValueExtractor( IdentityClaimTypes.AuthenticatedActorId, "AuthenticatedActorId" ),
                new ClaimValueExtractor( IdentityClaimTypes.ZoneId, "ZoneId" ),
                new ClaimValueExtractor( IdentityClaimTypes.XLCID, "XLCID" )
            };
        }

        public async Task<IdentityCheckResult> CheckIdentityAsync( IActivityMonitor monitor, ClaimsPrincipal principal, object model )
        {
            if( principal == null ) throw new ArgumentNullException( nameof( principal ) );
            if( model == null ) throw new ArgumentNullException( nameof( model ) );

            Type modelType = model.GetType();
            using( monitor.OpenTrace().Send( "Identity checking" ) )
            {
                monitor.Trace().Send( $"For type {modelType}" );

                IdentityCheckResult result = new IdentityCheckResult
                {
                    Success = true
                };
                IEnumerable<IValueExtractor> valueExtractors = GetAvailableExtractors();
                foreach( IValueExtractor extractor in valueExtractors )
                {
                    IValue value = extractor.ExtractValue( principal.Claims, model, modelType);
                    result.Success &= value.IsConsistent;
                    if( value.IsConsistent == false )
                    {
                        monitor.Info()
                            .Send( $"Extracted value was not consistent. Extracted={value.ExtractedValue}. Claim={value.ClaimValue}" );
                    }
                }
                await OnIdentityChecked( result );

                return result;
            }
        }


        protected virtual Task OnIdentityChecked( IdentityCheckResult result )
        {
            return Task.FromResult<object>( null );
        }

        protected virtual IEnumerable<IValueExtractor> GetAvailableExtractors()
        {
            return _defaultExtractors;
        }
    }

    public interface IValueExtractor
    {
        IValue ExtractValue( IEnumerable<Claim> claims, object model, Type modelType );
    }

    public interface IValue
    {
        bool IsConsistent { get; }

        object ExtractedValue { get; }

        object ClaimValue { get; }
    }

    public class ClaimValueExtractor : IValueExtractor
    {
        private readonly string _claimsType;
        private readonly string _propertyName;

        public ClaimValueExtractor( string claimsType, string propertyName )
        {
            _claimsType = claimsType;
            _propertyName = propertyName;
        }

        public IValue ExtractValue( IEnumerable<Claim> claims, object model, Type modelType )
        {
            Claim c = claims.FirstOrDefault( x => x.Type == _claimsType );
            if( c == null )
            {
                return DefaultValue.ClaimNotFoundValue;
            }
            var property = modelType.GetProperty( _propertyName );
            if( property == null )
            {
                return DefaultValue.PropertyNotFoundValue;
            }

            var propertyValue = property.GetGetMethod().Invoke( model, null );
            if( propertyValue == null )
            {
                return DefaultValue.NullValue;
            }
            return new DefaultValue( propertyValue, c );
        }

        private IValue ClaimNotFoundValue()
        {
            throw new NotImplementedException();
        }

        class DefaultValue : IValue
        {
            public static IValue ClaimNotFoundValue = new DefaultValue()
            {
                IsConsistent = true,
                ClaimValue = null,
                ExtractedValue = null
            };

            public static IValue PropertyNotFoundValue = new DefaultValue()
            {
                IsConsistent = true,
                ClaimValue = null,
                ExtractedValue = null
            };

            public static IValue NullValue = new DefaultValue()
            {
                IsConsistent = false,
                ClaimValue = null,
                ExtractedValue = null
            };

            private DefaultValue()
            {
            }

            public DefaultValue( object propertyValue, Claim c )
            {
                if( propertyValue == null ) throw new ArgumentNullException( nameof( propertyValue ) );
                if( c == null ) throw new ArgumentNullException( nameof( c ) );

                ExtractedValue = propertyValue;
                ClaimValue = GetValue( c.Value, c.ValueType, CultureInfo.InvariantCulture );
                IsConsistent = ClaimValue.Equals( ExtractedValue );
            }

            public object ClaimValue
            {
                get;
                private set;
            }

            public object ExtractedValue
            {
                get;
                private set;
            }

            public bool IsConsistent
            {
                get;
                private set;
            }
            public static object GetValue( string value, string valueType, IFormatProvider formatProvider )
            {
                switch( valueType )
                {
                    case ClaimValueTypes.String: return value.ToString( formatProvider );
                    case ClaimValueTypes.Integer:
                        short result;
                        short.TryParse( value, NumberStyles.Integer, formatProvider, out result );
                        return result;
                    case ClaimValueTypes.Integer32:
                        int result2;
                        int.TryParse( value, NumberStyles.Integer, formatProvider, out result2 );
                        return result2;
                    default: throw new NotSupportedException( $"ClaimsValueType {valueType} is not supported yet..." );
                }
            }
        }
    }
}
