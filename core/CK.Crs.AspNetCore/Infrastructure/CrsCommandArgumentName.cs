namespace CK.Crs.Infrastructure
{
    struct CrsCommandArgumentName
    {
        public CrsCommandArgumentName( string commandName )
        {
            Value = commandName;
        }

        public string Value { get; private set; }

        public static implicit operator string( CrsCommandArgumentName crsCommandName )
        {
            return crsCommandName.Value;
        }

        public static implicit operator CrsCommandArgumentName( string value )
        {
            return new CrsCommandArgumentName( value );
        }
    }

}
