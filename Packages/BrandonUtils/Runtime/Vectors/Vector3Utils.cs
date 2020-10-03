using UnityEngine;

namespace Packages.BrandonUtils.Runtime.Vectors {
    public static class Vector3Utils {
        public static Vector3 Copy(this Vector3 original) {
            return new Vector3(original.x, original.y, original.z);
        }

        /// <summary>
        /// Multiplies each component of <paramref name="original"/> by the corresponding component of <paramref name="scalar"/>.
        /// </summary>
        /// <remarks>
        /// This is similar to <see cref="Vector3.Scale(UnityEngine.Vector3)"/>, but returns a <b>new <see cref="Vector3"/></b> rather than modifying <paramref name="original"/>.
        /// </remarks>
        /// <param name="original"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Vector3 Scaled(this Vector3 original, Vector3 scalar) {
            return Vector3.Scale(original, scalar);
        }
    }
}