namespace BrandonUtils.Spatial {
    /**
     * Utilities for interacting with with cubes.
     *
     * <remarks>
     * May someday evolve into / be combined with a class that actually represents a cube itself.
     * </remarks>
     */
    public class Cube {
        /// <summary>
        /// Represents one <a href="https://en.wikipedia.org/wiki/Face_(geometry)">face</a> of a cube.
        /// </summary>
        public enum Face {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down
        }
    }
}
