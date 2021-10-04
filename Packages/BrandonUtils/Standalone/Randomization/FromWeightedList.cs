using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Randomization {
    public class FromWeightedList<T> : Randomized<T> {
        public readonly Func<T, double> WeightExtractor;

        public FromWeightedList(ICollection<T> choices, Func<T, double> weightExtractor, [CanBeNull] Random generator = default) : base(gen => Brandom.FromWeightedList(choices, weightExtractor, gen), generator) {
            this.WeightExtractor = weightExtractor;
        }

        public FromWeightedList(IDictionary<T, double> weightedChoices, [CanBeNull] Random generator = default) : base(gen => Brandom.FromWeightedList(weightedChoices, gen), generator) { }

        public FromWeightedList(IDictionary<T, int> weightedChoices, [CanBeNull] Random generator = default) : base(gen => Brandom.FromWeightedList(weightedChoices, gen), generator) { }

        public FromWeightedList(IDictionary<T, float> weightedChoices, [CanBeNull] Random generator = default) : base(gen => Brandom.FromWeightedList(weightedChoices, gen), generator) { }
    }
}