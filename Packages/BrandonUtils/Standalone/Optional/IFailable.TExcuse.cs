using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Represents something that <b>might</b> have failed.
    /// </summary>
    /// <remarks>
    /// <see cref="IFailable{TExcuse}"/> is the <b>base interface</b> for the various "failable" interfaces.
    /// </remarks>
    /// <typeparam name="TExcuse">the <see cref="Type"/> of the <see cref="Excuse"/></typeparam>
    [PublicAPI]
    public interface IFailable<out TExcuse> where TExcuse : Exception {
        /// <summary>
        /// The <see cref="TExcuse"/> <see cref="Exception"/> that caused this <see cref="IFailable{TExcuse}"/> to fail (if it did).
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>Retrieving <see cref="Excuse"/> when <see cref="Failed"/> is <c>false</c> should throw an <see cref="InvalidOperationException"/>.</li>
        /// <li><see cref="Excuse"/> should <b>never</b> return <c>null</c>.</li>
        /// </ul>
        /// </remarks>
        [NotNull]
        public TExcuse Excuse { get; }

        /// <summary>
        /// Whether or not this <see cref="IFailable{TExcuse}"/> was a failure.
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>If <see cref="Failed"/> is <c>true</c>, then <see cref="Excuse"/> should be present.</li>
        /// <li>If <see cref="Failed"/> is <c>false</c>, then <see cref="Excuse"/> should <b>not</b> be present.</li>
        /// </ul>
        /// </remarks>
        public bool Failed { get; }
    }
}