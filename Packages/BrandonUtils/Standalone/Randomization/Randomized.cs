using System;

namespace BrandonUtils.Standalone.Randomization {
    public class Randomized<T> : IRandomized<T> {
        public Func<T> Randomizer { get; protected set; }
        public T       Value      => Randomizer.Invoke();

        protected Randomized() { }

        public Randomized(Func<T> randomizer) {
            this.Randomizer = randomizer;
        }
    }
}
