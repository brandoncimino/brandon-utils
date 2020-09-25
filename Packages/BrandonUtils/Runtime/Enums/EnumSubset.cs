using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Packages.BrandonUtils.Runtime.Exceptions;

namespace Packages.BrandonUtils.Runtime.Enums {
    public class EnumSubset<T> : IList<T> where T : Enum {
        /// <summary>
        /// The backing <see cref="List{T}"/> for the <see cref="EnumSubset{T}"/>.
        /// </summary>
        private readonly List<T> _values;

        #region Constructors

        [UsedImplicitly]
        public EnumSubset(IEnumerable<T> values) {
            _values = values.ToList();
        }

        public EnumSubset(params T[] values) : this((IEnumerable<T>) values) { }

        #endregion

        #region IList Delegated Implementation

        public IEnumerator<T> GetEnumerator() {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _values.GetEnumerator();
        }

        public void Add(T item) {
            _values.Add(item);
        }

        public void Clear() {
            _values.Clear();
        }

        public bool Contains(T item) {
            return _values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _values.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return _values.Remove(item);
        }

        public int  Count      => _values.Count;
        public bool IsReadOnly { get; } = false;

        public int IndexOf(T item) {
            return _values.IndexOf(item);
        }

        public void Insert(int index, T item) {
            _values.Insert(index, item);
        }

        public void RemoveAt(int index) {
            _values.RemoveAt(index);
        }

        public T this[int index] {
            get => _values[index];
            set => _values[index] = value;
        }

        #endregion

        #region Meaningful Stuff

        /// <summary>
        /// Checks if <see cref="_values"/> <see cref="Contains"/> <paramref name="valuesToValidate"/>.
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