using UnityEngine;

namespace BrandonUtils.Vectors {
    public static class Vector2Utils {
        /// <summary>
        /// Returns a new <see cref="Vector2"/> that is a copy of <paramref name="original"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Vector2 Copy(this Vector2 original) {
            return new Vector2(original.x, original.y);
        }

        /// <summary>
        /// Multiplies each component of <paramref name="original"/> by the corresponding component of <paramref name="scale"/>.
        /// </summary>
        /// <remarks>
        /// This is similar to <see cref="Vector2.Scale(UnityEngine.Vector2)"/>, but returns a <b>new <see cref="Vector2"/></b> rather than modifying <paramref name="original"/>.
        /// </remarks>
        /// <param name="original"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        /// <seealso cref="Vector3Utils.Scaled"/>
        public static Vector2 Scaled(this Vector2 original, Vector2 scale) {
            return Vector2.Scale(original, scale);
        }
    }
}
