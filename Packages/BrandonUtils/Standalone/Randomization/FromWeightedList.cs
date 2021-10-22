using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Randomization {
    public class FromWeightedList<T> : Randomized<T> {
        public FromWeightedList(ICollection<T> choices, Func<T, double> weightExtractor, [CanBeNull] Random generator = default) : base(gen => Brandom.FromWeightedList(gen, choices, weightExtractor), generator) { }

        public FromWeightedList(IDictionary<T, double> weightedChoices, [CanBeNull] Random generator = default) : base(gen => Brandom.FromWeightedList(gen, weightedChoices), generator) { }
    }
}