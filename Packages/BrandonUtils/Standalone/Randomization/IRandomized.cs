using System;

namespace BrandonUtils.Standalone.Randomization {
    public interface IRandomized<out T> {
        public Func<T> Randomizer { get; }

        public T Value { get; }
    }
}
