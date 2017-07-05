namespace CK.Core
{
    public interface ITraitContextProvider
    {
        /// <summary>
        /// Gets the CRS <see cref="CKTraitContext"/>.
        /// </summary>
        CKTraitContext TraitContext { get; }
    }
}
