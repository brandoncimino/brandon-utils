using System;
using System.Collections.Generic;

namespace BrandonUtils.Standalone.Randomization {
    public class FromWeightedList<T> : Randomized<T> {
        public readonly Func<T, double> WeightExtractor;

        public FromWeightedList(ICollection<T> choices, Func<T, double> weightExtractor) : base(() => Brandom.FromWeightedList(choices, weightExtractor)) {
            this.WeightExtractor = weightExtractor;
        }

        public FromWeightedList(IDictionary<T, double> weightedChoices) : base(() => Brandom.FromWeightedList(weightedChoices)) { }

        public FromWeightedList(IDictionary<T, int> weightedChoices) : base(() => Brandom.FromWeightedList(weightedChoices)) { }

        public FromWeightedList(IDictionary<T, float> weightedChoices) : base(() => Brandom.FromWeightedList(weightedChoices)) { }
    }
}
