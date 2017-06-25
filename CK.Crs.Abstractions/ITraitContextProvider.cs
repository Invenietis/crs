using CK.Core;

namespace CK.Crs
{
    public interface ITraitContextProvider
    {
        /// <summary>
        /// Gets the CRS <see cref="CKTraitContext"/>.
        /// </summary>
        CKTraitContext TraitContext { get; }
    }
}
