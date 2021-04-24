using UnityEngine;

namespace BrandonUtils.Vectors {
    /// <summary>
    /// <a href="https://en.wikipedia.org/wiki/Syntactic_sugar">Syntactic sugar</a> for <b>built-in</b> <see cref="Vector3"/> methods.
    /// </summary>
    /// <remarks>
    /// Methods in <see cref="Vector3Sugar"/> should be extremely minimal wrappers around built-in methods,
    /// such as converting them to extension methods.
    ///
    /// Any method with <b>unique logic</b> should be placed into <see cref="Vector3Utils"/> instead.
    /// </remarks>
    public static class Vector3Sugar {
        /// <summary>
        /// <inheritdoc cref="Vector3.Lerp"/>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="lerpAmount"></param>
        /// <returns></returns>
        public static Vector3 Lerp(this Vector3 origin, Vector3 destination, float lerpAmount) {
            return Vector3.Lerp(origin, destination, lerpAmount);
        }
    }
}
