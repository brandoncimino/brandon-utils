using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Represents something that <i>might</i> have failed.
    /// </summary>
    /// <remarks>
    /// This is essentially an <see cref="Optional{T}"/>, except that it stores information about <i>why</i> the value isn't there in <see cref="Excuse"/>.
    /// </remarks>
    /// <typeparam name="TValue">The <see cref="IOptional{T}.Value"/>, if this succeeded</typeparam>
    /// <typeparam name="TExcuse">Information about the failure, if this <see cref="Failed"/></typeparam>
    [PublicAPI]
    public interface IFailable<out TValue, out TExcuse> : IOptional<TValue> {
        /// <summary>
        /// Information about why this <see cref="Failed"/> (if it did).
        /// </summary>
        /// <remarks>
        /// Retrieving this when the <see cref="IFailable{TValue,TExcuse}"/> wasn't <see cref="Failed"/> should throw an <see cref="InvalidOperationException"/>.
        /// </remarks>
        public TExcuse Excuse { get; }

        /// <summary>
        /// Whether or not this <see cref="IFailable{TValue,TExcuse}"/> was a failure.
        /// </summary>
        /// <remarks>
        /// Should always be the inverse of <see cref="IOptional{T}.HasValue"/>.
        /// </remarks>
        public bool Failed { get; }
    };
}