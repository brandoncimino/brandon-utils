using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Randomization {
    public class FromWeightedList<T> : Randomized<T> {
        public FromWeightedList(ICollection<T> choices, Func<T, double> weightExtractor, Random? generator = default) : base(gen => Brandom.FromWeightedList(gen, choices, weightExtractor), generator) { }

        public FromWeightedList(IDictionary<T, double> weightedChoices, Random?    generator = default) : base(gen => Brandom.FromWeightedList(gen, weightedChoices), generator) { }
        public FromWeightedList(IDictionary<T, int>    weightedChoices, Random? generator = default) : base(gen => Brandom.FromWeightedList(gen, weightedChoices), generator) { }

        public FromWeightedList(
            [NotNull] IEnumerable<(T, double)> weightedChoices,
            Random?                  generator = default
        ) : base(gen => gen.FromWeightedList(weightedChoices), generator) { }

        public FromWeightedList(
            [NotNull] IEnumerable<(T, int)> weightedChoices,
            Random?               generator = default
        ) : base(gen => gen.FromWeightedList(weightedChoices), generator) { }
    }
}