using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /**
     * <summary>A simplified <see cref="IFailableFunc{TValue,TExcuse}"/> that uses the base <see cref="Exception"/> type for its <see cref="Excuse"/>.</summary>
     * <inheritdoc cref="IFailableFunc{TValue,TExcuse}"/>
     */
    [PublicAPI]
    public readonly struct FailableFunc<TValue> : IFailableFunc<TValue, Exception>, IEquatable<IOptional<TValue>>, IEquatable<TValue> {
        public bool HasValue { get; }

        private readonly TValue _value;

        public TValue Value => HasValue ? _value : throw FailableException.FailedException(this);

        private readonly Exception _excuse;

        public Exception Excuse => _excuse ?? throw FailableException.DidNotFailException(this);

        public  bool                Failed                   => !HasValue;
        public  int                 Count                    => HasValue ? 1 : 0;
        private IEnumerable<TValue> EnumerableImplementation => HasValue ? Enumerable.Repeat(Value, 1) : Enumerable.Empty<TValue>();


        public FailableFunc(Func<TValue> valueSupplier) {
            try {
                _value   = valueSupplier.Invoke();
                HasValue = true;
                _excuse  = default;
            }
            catch (Exception e) {
                _value   = default;
                HasValue = false;
                _excuse  = e;
            }
        }

        #region Equality

        /// <summary>
        /// TODO: What does this dooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return (EqualityComparer<TValue>.Default.GetHashCode(_value) * 397) ^ HasValue.GetHashCode();
            }
        }

        public IEnumerator<TValue> GetEnumerator() {
            return EnumerableImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override bool Equals(object obj) {
            return obj switch {
                IOptional<TValue> optional => Equals(optional),
                TValue value               => Equals(value),
                _                          => Equals(this, obj)
            };
        }

        public bool Equals(IOptional<TValue> other) {
            return Optional.AreEqual(this, other);
        }

        public bool Equals(TValue other) {
            return Optional.AreEqual(this, other);
        }

        public static bool operator ==(FailableFunc<TValue> a, IOptional<TValue> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(FailableFunc<TValue> a, IOptional<TValue> b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(FailableFunc<TValue> a, TValue b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(FailableFunc<TValue> a, TValue b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(TValue a, FailableFunc<TValue> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(TValue a, FailableFunc<TValue> b) {
            return !Optional.AreEqual(a, b);
        }

        #endregion

        public override string ToString() {
            return Optional.ToString(this);
        }
    }
}