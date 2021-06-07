using UnityEngine;

namespace BrandonUtils.Vectors {
    public static class Vector3Utils {
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

        /// <summary>
        /// "Vectorally Interpolates" two <see cref="Vector3"/>s, i.e., <see cref="Mathf.Lerp"/>s each corresponding
        /// axis pair by the corresponding <paramref name="lerpAmounts"/> value.
        /// </summary>
        /// <example>
        /// <p>
        /// Example #1
        /// <br/>
        /// <c>result</c>.<see cref="Vector3.x"/> == <see cref="Mathf.Lerp">Mathf.Lerp</see>(<paramref name="original"/>.<see cref="Vector3.x"/>, <paramref name="destination"/>.<see cref="Vector3.x"/>, <see cref="lerpAmounts"/>.<see cref="Vector3.x"/>)
        /// </p>
        /// <br/>
        /// <p>
        /// Example #2
        /// <code>
        /// var orig = Vector3.zero;
        /// var dest = Vector3.one * 10;
        /// var lerp = new Vector3(0, 0.3, 99);
        ///
        /// Verp(orig, dest, lerp) == new Vector3(0, 3, 10);
        /// </code>
        /// </p>
        /// </example>
        /// <param name="original"></param>
        /// <param name="destination"></param>
        /// <param name="lerpAmounts"></param>
        /// <returns>a new <see cref="Vector3"/></returns>
        public static Vector3 Verp(this Vector3 original, Vector3 destination, Vector3 lerpAmounts) {
            return new Vector3(
                Mathf.Lerp(original.x, destination.x, lerpAmounts.x),
                Mathf.Lerp(original.y, destination.y, lerpAmounts.y),
                Mathf.Lerp(original.z, destination.z, lerpAmounts.z)
            );
        }

        #region Set Individual Axes

        /// <summary>
        /// Sets the <see cref="Vector3.x"/> coordinate of <paramref name="original"/>, returning <paramref name="original"/> for method chaining.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static ref Vector3 SetX(this ref Vector3 original, float x) {
            original.x = x;
            return ref original;
        }

        /// <summary>
        /// Sets the <see cref="Vector3.y"/> coordinate of <see cref="original"/>, returning <see cref="original"/> for method chaining.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static ref Vector3 SetY(this ref Vector3 original, float y) {
            original.y = y;
            return ref original;
        }

        /// <summary>
        /// Sets the <see cref="Vector3.z"/> coordinate of <see cref="original"/>, returning <see cref="original"/> for method chaining.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static ref Vector3 SetZ(this ref Vector3 original, float z) {
            original.z = z;
            return ref original;
        }

        #endregion
    }
}
