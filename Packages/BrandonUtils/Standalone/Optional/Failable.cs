using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /**
     * <inheritdoc cref="IFailable{TValue,TExcuse}"/>
     */
    [PublicAPI]
    public readonly struct Failable<TValue, TExcuse> : IFailable<TValue, TExcuse>, IEquatable<TValue>, IEquatable<IOptional<TValue>> {
        public bool HasValue { get; }

        private readonly TValue _value;

        public TValue Value => HasValue ? _value : throw new InvalidOperationException($"Unable to retrieve the {typeof(TValue).Name} {nameof(Value)} from the {nameof(Failable<TValue, TExcuse>)} because it failed!\n{nameof(Excuse)}: {Excuse}");

        public bool Failed => !HasValue;

        private readonly TExcuse _excuse;

        public TExcuse Excuse => Failed ? _excuse : throw new InvalidOperationException($"Unable to retrieve the {nameof(Excuse)} of type {typeof(TExcuse).Name} from the {GetType().Name} because it didn't fail! (Actual {nameof(Value)}: {Value})");

        /// <summary>
        /// The basic constructor for <see cref="Failable{TValue,TExcuse}"/> instances.
        /// </summary>
        /// <remarks>
        /// Using the <see cref="Func{TResult}"/> extension method <see cref="Optional.Try{T}"/> is often more convenient than constructing <see cref="Failable{TValue,TExcuse}"/>s directly.
        /// </remarks>
        /// <param name="valueSupplier"></param>
        /// <param name="disclaimer"></param>
        /// <seealso cref="Optional.Try{T}"/>
        public Failable(Func<TValue> valueSupplier, Func<Exception, TExcuse> disclaimer) {
            try {
                _value   = valueSupplier.Invoke();
                HasValue = true;
                _excuse  = default;
            }
            catch (Exception e) {
                _value   = default;
                HasValue = false;
                _excuse  = disclaimer.Invoke(e);
            }
        }

        #region Equality

        public bool Equals(TValue other) {
            return Optional.AreEqual(this, other);
        }

        public bool Equals(IOptional<TValue> other) {
            return Optional.AreEqual(this, other);
        }

        public static bool operator ==(Failable<TValue, TExcuse> a, IOptional<TValue> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(Failable<TValue, TExcuse> a, IOptional<TValue> b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(Failable<TValue, TExcuse> a, Optional<TValue> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(Failable<TValue, TExcuse> a, Optional<TValue> b) {
            return !Optional.AreEqual(a, b);
        }

        #region Jetbrains auto-generated equality members

        public bool Equals(Failable<TValue, TExcuse> other) {
            return EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object obj) {
            return obj is Failable<TValue, TExcuse> other && Equals(other);
        }

        public override int GetHashCode() {
            return EqualityComparer<TValue>.Default.GetHashCode(_value);
        }

        #endregion

        #endregion

        public override string ToString() {
            return Optional.ToString(this);
        }
    }

    /**
     * <summary>A simplified <see cref="Failable{TValue,TExcuse}"/> that uses the base <see cref="Exception"/> type for its <see cref="Excuse"/>.</summary>
     * <inheritdoc cref="IFailable{TValue,TExcuse}"/>
     */
    [PublicAPI]
    public readonly struct Failable<TValue> : IFailable<TValue, Exception>, IEquatable<IOptional<TValue>>, IEquatable<TValue> {
        public override bool Equals(object obj) {
            return obj switch {
                IOptional<TValue> optional => Equals(optional),
                TValue value               => Equals(value),
                _                          => Equals(this, obj)
            };
        }

        /// <summary>
        /// TODO: What does this dooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return (EqualityComparer<TValue>.Default.GetHashCode(_value) * 397) ^ HasValue.GetHashCode();
            }
        }

        public           bool      HasValue { get; }
        private readonly TValue    _value;
        public           TValue    Value => HasValue ? _value : throw new InvalidOperationException($"Unable to retrieve the {typeof(TValue).Name} {nameof(Value)} from the {GetType().Name} because it failed!", Excuse);
        private readonly Exception _excuse;
        public           Exception Excuse => Failed ? _excuse : throw new InvalidOperationException($"Unable to retrieve the {nameof(Excuse)} from the {GetType().Name} because it succeeded! (Actual {nameof(Value)}: {Value})");
        public           bool      Failed => !HasValue;


        public Failable(Func<TValue> valueSupplier) {
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

        public bool Equals(IOptional<TValue> other) {
            return Optional.AreEqual(this, other);
        }

        public bool Equals(TValue other) {
            return Optional.AreEqual(this, other);
        }

        public static bool operator ==(Failable<TValue> a, IOptional<TValue> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(Failable<TValue> a, IOptional<TValue> b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(Failable<TValue> a, TValue b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(Failable<TValue> a, TValue b) {
            return !Optional.AreEqual(a, b);
        }

        public static bool operator ==(TValue a, Failable<TValue> b) {
            return Optional.AreEqual(a, b);
        }

        public static bool operator !=(TValue a, Failable<TValue> b) {
            return !Optional.AreEqual(a, b);
        }

        #endregion

        public override string ToString() {
            return Optional.ToString(this);
        }
    }
}