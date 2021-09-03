﻿using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// A mockery of Java's <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html">Optional</a> class.
    /// </summary>
    /// <remarks>
    /// After an amount of deliberation, on 9/2/2021 I have decided that an <see cref="IOptional{T}"/> must, by default, assume that <see cref="Value"/> <see cref="CanBeNullAttribute"/>.
    ///
    /// Specific implementations might be more strict; for example, I think there may be value in having <see cref="Optional{T}"/>, the primary implementation, never return null.
    ///
    /// With regards to "allowing <c>null</c> as a valid <see cref="Value"/> in <see cref="IOptional{T}"/>":
    /// <ul><b>Pro</b>
    /// <li>"safest" option - accounting for null when it is impossible is OK; not accounting for null when is is possible will cause exceptions</li>
    /// <li>the delegate / functional interface options can definitely return null (<see cref="IFailable"/>, <see cref="IFailableFunc{TValue}"/>, etc.)</li>
    /// </ul>
    /// <ul><b>Con</b>
    /// <li>Ambiguous comparisons between:
    /// <ul>
    /// <li>A null <see cref="IOptional{T}"/></li>
    /// <li>An <see cref="IOptional{T}"/> <b><i>containing</i></b> a null <typeparamref name="T"/></li>
    /// <li>A null <typeparamref name="T"/></li>
    /// <li><see cref="IOptional{T}"/>s with different <typeparamref name="T"/> types that both contain null</li>
    /// </ul>
    /// </li>
    /// </ul>
    /// </remarks>
    /// <typeparam name="T">the type of the <see cref="Value"/> that <i>might</i> be there.</typeparam>
    public interface IOptional<out T> : IReadOnlyCollection<T> {
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
        [CanBeNull]
        T Value { get; }
    }
}