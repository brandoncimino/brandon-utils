using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Packages.BrandonUtils.Runtime.Exceptions;

namespace Packages.BrandonUtils.Runtime.Enums {
    /// <summary>
    /// A wrapper around a <see cref="IList{T}"/> of <see cref="Enum"/> <typeparamref name="T"/> values.
    /// <br/>
    /// Use this to "restrict" an existing <see cref="Enum"/>.
    /// </summary>
    /// <example>
    /// <see cref="EnumSubset{T}"/> Weekends = new <see cref="EnumSubset{T}"/>(<see cref="DayOfWeek.Saturday"/>, <see cref="DayOfWeek.Sunday"/>);
    /// <p/>
    /// Weekends.<see cref="Validate"/>(<see cref="DayOfWeek.Monday"/>); //this throws an <see cref="EnumNotInSubsetException{T}"/>
    /// </example>
    /// <typeparam name="T"></typeparam>
    public class EnumSubset<T> : ICollection<T> where T : Enum {
        /// <summary>
        /// The backing <see cref="IList{T}"/> for the <see cref="EnumSubset{T}"/>.
        /// </summary>
        private readonly IList<T> Subset;

        #region Constructors

        public EnumSubset(IList<T> values, bool isReadOnly = false) {
            Subset = isReadOnly ? new ReadOnlyCollection<T>(values) : values;
        }

        public EnumSubset(params T[] values) : this(values, false) { }

        public EnumSubset(IEnumerable<IEnumerable<T>> values, bool isReadOnly = false) : this(values.SelectMany(it => it).ToList(), isReadOnly) { }

        #endregion

        #region IList Delegated Implementation

        public IEnumerator<T> GetEnumerator() {
            return Subset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Subset.GetEnumerator();
        }

        public void Add(T item) {
            Subset.Add(item);
        }

        public void Clear() {
            Subset.Clear();
        }

        public bool Contains(T item) {
            return Subset.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Subset.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return Subset.Remove(item);
        }

        public int  Count      => Subset.Count;
        public bool IsReadOnly => Subset.IsReadOnly;

        public int IndexOf(T item) {
            return Subset.IndexOf(item);
        }

        public void Insert(int index, T item) {
            Subset.Insert(index, item);
        }

        public void RemoveAt(int index) {
            Subset.RemoveAt(index);
        }

        public T this[int index] {
            get => Subset[index];
            set => Subset[index] = value;
        }

        #endregion

        #region Meaningful Stuff

        /// <summary>
        /// Checks if <see cref="Subset"/> <see cref="Contains"/> <paramref name="valuesToValidate"/>.
        /// </summary>
        /// <remarks>
        /// This should only be used when you want strict validation that throws an <see cref="Exception"/> on failure.
        /// <br/>If you want to "test" whether or not a value is in an <see cref="EnumSubset{T}"/>, use <see cref="Contains"/>.
        /// </remarks>
        /// <param name="valuesToValidate">The <see cref="T"/> <see cref="Enum"/> value that should be within this <see cref="EnumSubset{T}"/>.</param>
        /// <returns><paramref name="valuesToValidate"/>, if <see cref="Contains">Contains</see>(<paramref name="valuesToValidate"/>) returns <c>true</c>.</returns>
        /// <exception cref="EnumNotInSubsetException{T}">if <see cref="Contains">Contains</see>(<paramref name="valuesToValidate"/>) returns <c>false</c>.</exception>
        public void Validate(params T[] valuesToValidate) {
            if (!valuesToValidate.All(Contains)) {
                throw new EnumNotInSubsetException<T>(
                    this,
                    valuesToValidate
                );
            }
        }

        #endregion
    }
}