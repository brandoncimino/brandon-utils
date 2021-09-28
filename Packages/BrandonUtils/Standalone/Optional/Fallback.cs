using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// A "box" for a value that that <i>might</i> have an <see cref="ExplicitValue"/>, but otherwise, has a <see cref="FallbackValue"/>.
    /// </summary>
    /// <remarks>
    /// Can also be thought of as a "default" and "override", i.e.:
    /// <ul>
    /// <li><see cref="FallbackValue"/> -> "default"</li>
    /// <li><see cref="ExplicitValue"/> -> "override"</li>
    /// </ul>
    /// </remarks>
    /// <typeparam name="T">the underlying value's type</typeparam>
    [PublicAPI]
    public class Fallback<T> : IOptional<T> {
        /// <summary>
        /// The <see cref="Optional{T}"/> value that might have been <see cref="Set"/>.
        /// </summary>
        public Optional<T> ExplicitValue { get; private set; }
        /// <summary>
        /// Whether or not <see cref="ExplicitValue"/> has been <see cref="Set"/>.
        /// </summary>
        public bool HasValue => ExplicitValue.HasValue;

        /// <summary>
        /// <see cref="ExplicitValue"/>, if present; otherwise, <see cref="FallbackValue"/>.
        /// </summary>
        public T Value {
            get => ExplicitValue.GetValueOrDefault(FallbackValue);
            set => ExplicitValue = value;
        }

        /// <summary>
        /// The value returned when <see cref="ExplicitValue"/> isn't present.
        /// </summary>
        [CanBeNull]
        public T FallbackValue { get; }

        public int Count => ExplicitValue.Count;

        public Fallback([CanBeNull] T fallbackValue = default) {
            FallbackValue = fallbackValue;
        }

        /// <summary>
        /// Sets the <see cref="ExplicitValue"/>'s <see cref="Optional{T}.Value"/>.
        /// </summary>
        /// <remarks>
        /// Passing <paramref name="explicitValue"/> as <c>null</c> will result in <see cref="ExplicitValue"/> being an <see cref="Optional{T}"/> that <i>contains</i> <c>null.</c>
        ///
        /// If you want to <b>remove</b> the <see cref="ExplicitValue"/>, so that <see cref="FallbackValue"/> is returned instead, use <see cref="Unset"/>.
        /// </remarks>
        /// <param name="explicitValue">the new <see cref="ExplicitValue"/>'s <see cref="Optional{T}.Value"/></param>
        /// <returns>this, for method chaining</returns>
        [NotNull]
        public Fallback<T> Set([CanBeNull] T explicitValue) {
            ExplicitValue = new Optional<T>(explicitValue);
            return this;
        }

        /// <summary>
        /// Sets <see cref="ExplicitValue"/> to an <b>empty</b> <see cref="Optional{T}"/>.
        /// </summary>
        /// <returns>this, for method chaining</returns>
        [NotNull]
        public Fallback<T> Unset() {
            ExplicitValue = new Optional<T>();
            return this;
        }

        public static implicit operator T(Fallback<T> self) {
            return self.Value;
        }

        #region GetEnumerator()

        public IEnumerator<T> GetEnumerator() {
            return ExplicitValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)ExplicitValue).GetEnumerator();
        }

        #endregion
    }
}