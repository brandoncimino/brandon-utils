using System;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// A mockery of Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html">Optional</a> class.
    /// </summary>
    /// <typeparam name="T">the type of the <see cref="Value"/> that <i>might</i> be there.</typeparam>
    public interface IOptional<out T> {
        /// <summary>
        /// Whether or not a <see cref="Value"/> is present.
        /// </summary>
        /// <remarks>
        /// Corresponds to:
        /// <ul>
        /// <li><see cref="Nullable{T}"/>.<see cref="Nullable{T}.HasValue"/></li>
        /// <li>Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html#isPresent--">Optional.isPresent()</a></li>
        /// </ul>
        /// </remarks>
        bool HasValue { get; }

        /// <summary>
        /// The actual <see cref="T"/> in this <see cref="IOptional{T}"/>, if present.
        /// </summary>
        /// <remarks>
        /// Retrieving this value while <see cref="HasValue"/> is false should throw an <see cref="IndexOutOfRangeException"/>.
        /// </remarks>
        T Value { get; }
    }
}