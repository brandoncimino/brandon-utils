using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// A simplified <see cref="IFailable{TExcuse}"/> that uses the base <see cref="Exception"/> type.
    /// </summary>
    /// <inheritdoc cref="IFailable{TExcuse}"/>
    [PublicAPI]
    public interface IFailable : IFailable<Exception> { }
}