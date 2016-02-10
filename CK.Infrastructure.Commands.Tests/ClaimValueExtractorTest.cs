using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace CK.Infrastructure.Commands.Tests
{
    public class ClaimValueExtractorTest
    {
        class ModelWithProperty
        {
            public object MyIntegerClaim { get; set; }
        }

        [Theory]
        [InlineData( 1, "1", ClaimValueTypes.Integer32, true )]
        [InlineData( 110, "0110", ClaimValueTypes.Integer32, true )]
        [InlineData( 12, "122", ClaimValueTypes.Integer32, false )]
        [InlineData( "1", "1", ClaimValueTypes.String, true )]
        [InlineData( "110", "0110", ClaimValueTypes.String, false )]
        [InlineData( "12", "122", ClaimValueTypes.String, false )]
        [InlineData( (short)12, "122", ClaimValueTypes.Integer, false )]
        [InlineData( (short)12, "12", ClaimValueTypes.Integer, true )]
        public void Extracted_Values_And_Claim_Values_Consitency_Test( object value, string claimValue, string claimValueType, bool shouldBeConsistent )
        {
            ClaimValueExtractor e = new ClaimValueExtractor( "MyIntegerClaim", "MyIntegerClaim");
            ModelWithProperty model = Activator.CreateInstance<ModelWithProperty>();
            model.MyIntegerClaim = value;

            IValue v = e.ExtractValue( GetClaims( claimValue, claimValueType), model, model.GetType() );
            Assert.Equal( shouldBeConsistent, v.IsConsistent );
        }

        public IEnumerable<Claim> GetClaims( string value, string claimValueType )
        {
            yield return new Claim( "MyIntegerClaim", value, claimValueType );
        }
    }
}
