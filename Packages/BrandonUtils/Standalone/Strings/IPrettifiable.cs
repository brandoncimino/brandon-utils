using System.Diagnostics.Contracts;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Strings {
    /// <summary>
    /// Allows an object to specifically designate a method for use by <see cref="Prettification"/>.
    /// </summary>
    public interface IPrettifiable {
        [NotNull]
        [Pure]
        public string Prettify([CanBeNull] PrettificationSettings settings = default);
    }
}