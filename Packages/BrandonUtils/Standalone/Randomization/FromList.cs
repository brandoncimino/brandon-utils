using System;
using System.Collections.Generic;

using BrandonUtils.Standalone.Collections;

namespace BrandonUtils.Standalone.Randomization {
    public class FromList<T> : List<T>, IRandomized<T> {
        public Func<T> Randomizer => this.Random;
        public T       Value      => Randomizer.Invoke();
    }
}
