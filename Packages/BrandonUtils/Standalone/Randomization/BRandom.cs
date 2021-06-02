using System;

namespace BrandonUtils.Standalone.Randomization {
    public static class BRandom {
        public static readonly Random Gen = new Random();

        /// <summary>
        /// Returns a random sign (either 1 or -1)
        /// </summary>
        /// <returns></returns>
        public static int Sign() {
            return Gen.Next(2) == 0 ? -1 : 1;
        }

        public static double Near(double center, double radius) {
            return center + Gen.NextDouble() * radius * Sign();
        }
    }
}
