using BrandonUtils.Vectors;

using UnityEngine;

namespace BrandonUtils.Standalone.Randomization {
    public class InRange : Randomized<float> {
        public Vector2 Range;

        public InRange(Vector2 range) {
            this.Range      = range;
            this.Randomizer = () => range.Random();
        }
    }
}
