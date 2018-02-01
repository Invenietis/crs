namespace CK.Crs
{
    public interface IFeatureContainer
    {
        /// <summary>
        /// Gets the feature of the given type or null if the feature does not exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetFeature<T>() where T : class;

        /// <summary>
        /// Sets a feature for the given type. Pass null to remove a feature.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="feature"></param>
        void SetFeature<T>( T feature ) where T : class;
    }
}
