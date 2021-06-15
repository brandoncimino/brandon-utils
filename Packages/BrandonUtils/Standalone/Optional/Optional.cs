using System;

namespace BrandonUtils.Standalone.Optional {
    public static class Optional {
        public static Optional<T> Of<T>(T value) {
            return new Optional<T>(value);
        }

        public static Optional<T> Empty<T>() {
            return new Optional<T>();
        }
    }

    /// <summary>
    /// A mockery of Java's Optional class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Optional<T> {
        private readonly T _value;

        public T Value => HasValue ? _value : throw new ArgumentOutOfRangeException($"Attempted to retrieve an {nameof(Optional<T>)} {nameof(Value)} of type {typeof(T)}, but none is present!");

        public bool HasValue { get; }

        public Optional(T value) {
            if (value == null) {
                HasValue = false;
                _value   = default;
                return;
            }

            HasValue = true;
            _value   = value;
        }

        public Optional<TNew> Map<TNew>(Func<T, TNew> mappingFunction) {
            return HasValue ? new Optional<TNew>(mappingFunction.Invoke(_value)) : new Optional<TNew>();
        }

        public static implicit operator Optional<T>(T value) {
            return new Optional<T>(value);
        }

        public static implicit operator T(Optional<T> optional) {
            return optional.Value;
        }
    }
}