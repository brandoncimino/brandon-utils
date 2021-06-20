using System;
using System.Collections.Generic;

using BrandonUtils.Standalone.Optional;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Represents something that <i>might</i> have failed.
    /// </summary>
    /// <remarks>
    /// This is essentially an <see cref="Optional{T}"/>, except that it stores information about <i>why</i> the value isn't there in <see cref="Excuse"/>.
    /// </remarks>
    /// <typeparam name="TValue">The <see cref="IOptional{T}.Value"/>, if this succeeded</typeparam>
    /// <typeparam name="TExcuse">Information about the failure, if this <see cref="Failed"/></typeparam>
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

/**
 * <inheritdoc cref="IFailable{TValue,TExcuse}"/>
 */
public readonly struct Failable<TValue, TExcuse> : IFailable<TValue, TExcuse>, IEquatable<TValue>, IEquatable<IOptional<TValue>> {
    public           bool   HasValue { get; }
    private readonly TValue _value;
    public           TValue Value => HasValue ? _value : throw new InvalidOperationException($"Unable to retrieve the {typeof(TValue).Name} {nameof(Value)} from the {nameof(Failable<TValue, TExcuse>)} because it failed!\n{nameof(Excuse)}: {Excuse}");

    public           bool    Failed => !HasValue;
    private readonly TExcuse _excuse;
    public           TExcuse Excuse => Failed ? _excuse : throw new InvalidOperationException($"Unable to retrieve the {nameof(Excuse)} of type {typeof(TExcuse).Name} from the {GetType().Name} because it didn't fail! (Actual {nameof(Value)}: {Value})");

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

    #endregion

    public override string ToString() {
        return Optional.ToString(this);
    }
}

public readonly struct Failable<TValue> : IFailable<TValue, Exception>, IEquatable<IOptional<TValue>>, IEquatable<TValue> {
    public override bool Equals(object obj) {
        switch (obj) {
            case IOptional<TValue> optional:
                return Equals(optional);
            case TValue value:
                return Equals(value);
            default:
                return Equals(this, obj);
        }
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