using System;

namespace CK.Crs
{
    class Feature
    {
        private object _features;

        class FeatureItem
        {
            public Type Type { get; set; }

            public object Item { get; set; }

            FeatureItem()
            {
            }

            public static FeatureItem Create<T>( object item )
            {
                return new FeatureItem
                {
                    Item = item,
                    Type = typeof( T )
                };
            }
        }

        public T GetFeature<T>() where T : class
        {
            if( _features == null ) return null;

            if( _features is FeatureItem[] featureArray )
            {
                for( int i = 0; i < featureArray.Length; ++i )
                {
                    if( featureArray[i].Type == typeof( T ) ) return featureArray[i].Item as T;
                }
            }

            if( _features is FeatureItem featureItem && featureItem.Type == typeof( T ) )
            {
                return featureItem.Item as T;
            }

            return null;
        }

        public void RemoveFeature<T>() where T : class
        {
            var feature = GetFeature<T>();
            if( feature == null ) return; // Ignores silently there is no feature with the given type registered.

            if( _features is FeatureItem[] featureArray )
            {
                var newFeatureArray = new FeatureItem[featureArray.Length - 1];
                int j = 0;
                for( int i = 0; i < featureArray.Length; ++i )
                {
                    var theFeature = featureArray[i];
                    if( theFeature.Type != typeof( T ) ) newFeatureArray[j++] = theFeature;
                }
                _features = newFeatureArray;
            }
            if( _features is FeatureItem featureItem && featureItem.Type == typeof( T ) )
            {
                _features = null;
            }
        }

        public void SetFeature<T>( T feature ) where T : class
        {
            if( feature == null )
            {
                throw new ArgumentNullException( nameof( feature ) );
            }

            if( _features == null ) _features = FeatureItem.Create<T>( feature );
            else
            {
                if( _features is FeatureItem[] featureArray )
                {
                    if( GetFeature<T>() != null ) throw new ArgumentException( "A feature with the given type is already registered" );

                    Array.Resize( ref featureArray, featureArray.Length + 1 );
                    featureArray[featureArray.Length - 1] = FeatureItem.Create<T>( feature );
                    _features = featureArray;
                }
                else
                {
                    _features = new FeatureItem[2] { (FeatureItem)_features, FeatureItem.Create<T>( feature ) };
                }
            }
        }
    }
}
