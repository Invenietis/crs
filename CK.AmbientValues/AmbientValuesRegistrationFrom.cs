using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CK.Core
{
    internal class AmbientValuesRegistrationFrom<T> : IAmbientValuesRegistrationFrom<T>
    {
        private IAmbientValuesRegistration _registration;

        public AmbientValuesRegistrationFrom( IAmbientValuesRegistration registration )
        {
            _registration = registration;
        }

        public IAmbientValuesProviderConfiguration<T> Select<TProperty>( Expression<Func<T, TProperty>> propertyLambda ) where TProperty : IComparable
        {
            Type type = typeof( T );

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if( member == null )
                throw new ArgumentException( string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString() ) );

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if( propInfo == null )
                throw new ArgumentException( string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString() ) );

            return new AmbientValuesProviderConfiguration<T>( _registration, this, propInfo );
        }
    }
}