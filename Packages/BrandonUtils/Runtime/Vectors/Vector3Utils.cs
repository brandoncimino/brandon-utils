using UnityEngine;

namespace Packages.BrandonUtils.Runtime.Vectors {
    public static class Vector3Utils {
        public static Vector3 Copy(this Vector3 original) {
            return new Vector3(original.x, original.y, original.z);
        }

        /// <summary>
        /// Multiplies each component of <paramref name="original"/> by the corresponding component of <paramref name="scale"/>.
        /// </summary>
        /// <remarks>
        /// This is similar to <see cref="Vector3.Scale(UnityEngine.Vector3)"/>, but returns a <b>new <see cref="Vector3"/></b> rather than modifying <paramref name="original"/>.
        /// </remarks>
        /// <param name="original"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        /// <seealso cref="Vector2Utils.Scaled"/>
        public static Vector3 Scaled(this Vector3 original, Vector3 scale) {
            return Vector3.Scale(original, scale);
        }

        #region Aviation Axes

        /// <summary>
        /// An "alias" for <paramref name="eulerAngles"/>.<see cref="Vector3.x"/> which returns the <a href="https://en.wikipedia.org/wiki/Aircraft_principal_axes#Transverse_axis_(pitch)">aviation "pitch" (transverse)</a> axis.
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public static float Pitch(this Vector3 eulerAngles) {
            return eulerAngles.x;
        }

        /// <summary>
        /// An "alias" for <paramref name="eulerAngles"/>.<see cref="Vector3.y"/> which returns the <a href="https://en.wikipedia.org/wiki/Aircraft_principal_axes#Vertical_axis_(yaw)">aviation "yaw" (vertical)</a> axis.
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public static float Yaw(this Vector3 eulerAngles) {
            return eulerAngles.y;
        }

        /// <summary>
        /// An "alias" for <paramref name="eulerAngles"/>.<see cref="Vector3.y"/> which returns the <a href="https://en.wikipedia.org/wiki/Aircraft_principal_axes#Longitudinal_axis_(roll)">aviation "roll" (longitudinal)</a> axis.
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public static float Roll(this Vector3 eulerAngles) {
            return eulerAngles.z;
        }

        #endregion
    }
}