using UnityEngine;

namespace BrandonUtils.Vectors {
    public static class Vector2Utils {
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

        #region Set Individual Axes

        /// <summary>
        /// Updates the <see cref="Vector2.x"/> coordinate of <paramref name="original"/>, and returns <paramref name="original"/>.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static ref Vector2 SetX(this ref Vector2 original, float x) {
            original.x = x;
            return ref original;
        }

        /// <inheritdoc cref="SetX(ref UnityEngine.Vector2,float)"/>
        public static ref Vector2Int SetX(this ref Vector2Int original, int x) {
            original.x = x;
            return ref original;
        }

        /// <summary>
        /// Updates the <see cref="Vector2.y"/> coordinate of <paramref name="original"/>, and returns <paramref name="original"/>
        /// </summary>
        /// <param name="original"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static ref Vector2 SetY(this ref Vector2 original, float y) {
            original.y = y;
            return ref original;
        }

        /**
         * <inheritdoc cref="SetY(ref UnityEngine.Vector2,float)"/>
         */
        public static ref Vector2Int SetY(this ref Vector2Int original, int y) {
            original.y = y;
            return ref original;
        }

        #endregion

        /// <summary>
        /// Returns the <see cref="Mathf.Min(float,float)"/> of the <see cref="Vector2"/>'s coordinates.
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static float Min(this Vector2 vector2) {
            return Mathf.Min(vector2.x, vector2.y);
        }

        /**
         * <inheritdoc cref="Min(UnityEngine.Vector2)"/>
         */
        public static int Min(this Vector2Int vector2Int) {
            return Mathf.Min(vector2Int.x, vector2Int.y);
        }


        /// <summary>
        /// Returns the <see cref="Mathf.Max(float,float)"/> of the <see cref="Vector2"/>'s coordinates.
        /// </summary>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static float Max(this Vector2 vector2) {
            return Mathf.Max(vector2.x, vector2.y);
        }

        /**
         * <inheritdoc cref="Max(UnityEngine.Vector2)"/>
         */
        public static int Max(this Vector2Int vector2Int) {
            return Mathf.Max(vector2Int.x, vector2Int.y);
        }

        /// <summary>
        /// Returns a <b>new</b> <see cref="Vector2"/> containing the values from <paramref name="original"/> sorted in ascending order (i.e. <see cref="Vector2.x"/> <![CDATA[<=]]> <see cref="Vector2.y"/>)
        /// </summary>
        /// <remarks>
        /// This will always return a <b>new <see cref="Vector2"/></b>, even if <paramref name="original"/> was already sorted.
        /// </remarks>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Vector2 Sorted(this Vector2 original) {
            return original.x > original.y ? original.Swapped() : original;
        }

        /**
         * <inheritdoc cref="Sorted(UnityEngine.Vector2)"/>
         */
        public static Vector2Int Sorted(this Vector2Int original) {
            return original.x > original.y ? original.Swapped() : original;
        }

        /// <summary>
        /// Arranges <paramref name="original"/>'s <see cref="Vector2.x"/> and <see cref="Vector2.y"/> so that <![CDATA[x <= y]]>.
        /// </summary>
        /// <remarks>This method <b>modifies <paramref name="original"/></b>!</remarks>
        /// <param name="original"></param>
        /// <returns></returns>
        public static ref Vector2 Sort(this ref Vector2 original) {
            if (original.x <= original.y) {
                return ref original;
            }

            var old_x = original.x;
            var old_y = original.y;
            original.x = old_y;
            original.y = old_x;
            return ref original;
        }

        /// <summary>
        /// Similar to <see cref="Swap"/>, but returns a <b>new</b> <see cref="Vector2"/>.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector2 Swapped(this Vector2 self) {
            return new Vector2(self.y, self.x);
        }

        /**
         * <inheritdoc cref="Swapped(UnityEngine.Vector2)"/>
         */
        public static Vector2Int Swapped(this Vector2Int self) {
            return new Vector2Int(self.y, self.x);
        }

        /// <summary>
        /// Switches the values the <paramref name="self"/>'s <see cref="Vector2.x"/> and <see cref="Vector2.y"/>.
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static ref Vector2 Swap(this ref Vector2 self) {
            var old_x = self.x;
            var old_y = self.y;
            self.x = old_y;
            self.y = old_x;
            return ref self;
        }

        /**
         * <inheritdoc cref="Swap(ref UnityEngine.Vector2)"/>
         */
        public static ref Vector2Int Swap(this ref Vector2Int self) {
            var old_x = self.x;
            var old_y = self.y;
            self.x = old_y;
            self.y = old_x;
            return ref self;
        }

        /// <summary>
        /// Returns a random number between <paramref name="rangeInclusive"/>.<see cref="Vector2.x"/> and <see cref="Vector2.y"/>.
        /// </summary>
        /// <remarks>
        /// Unity's <see cref="UnityEngine.Random.Range(float,float)"/> automatically sorts the min and max values, so there is no need to call <see cref="Sorted(UnityEngine.Vector2)"/>.
        /// </remarks>
        /// <param name="rangeInclusive"></param>
        /// <returns></returns>
        public static float Random(this Vector2 rangeInclusive) {
            return UnityEngine.Random.Range(rangeInclusive.x, rangeInclusive.y);
        }

        /// <summary>
        /// Returns a random <see cref="UnityEngine.Random.Range(int,int)"/> value between <see cref="intRange"/>.<see cref="Vector2Int.x"/> and <see cref="Vector2Int.y"/>.
        /// </summary>
        /// <remarks>
        /// Unity's <see cref="UnityEngine.Random.Range(int,int)"/> automatically sorts the min and max values of <see cref="intRange"/>, so there is no need to call <see cref="Sorted(UnityEngine.Vector2Int)"/>.
        /// </remarks>
        /// <param name="intRange"></param>
        /// <returns></returns>
        public static int Random(this Vector2Int intRange) {
            return UnityEngine.Random.Range(intRange.x, intRange.y);
        }

        /// <summary>
        /// Returns whether or not <paramref name="value"/> is within <paramref name="range"/>'s <see cref="Min(UnityEngine.Vector2)"/> and <see cref="Max(UnityEngine.Vector2)"/>, <b>inclusively</b>.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Contains(this Vector2 range, float value) {
            return range.Min() <= value && value <= range.Max();
        }

        /**
         * <inheritdoc cref="Contains(UnityEngine.Vector2,float)"/>
         */
        public static bool Contains(this Vector2Int range, int value) {
            return range.Min() <= value && value <= range.Max();
        }

        /// <summary>
        /// Performs a <see cref="Mathf.Lerp"/> between <paramref name="range"/>'s <see cref="Vector2.x"/> and <see cref="Vector2.y"/>.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="lerpAmount"></param>
        /// <returns></returns>
        public static float Lerp(this Vector2 range, float lerpAmount) {
            return Mathf.Lerp(range.x, range.y, lerpAmount);
        }

        /// <summary>
        /// Performs a <see cref="Mathf.LerpUnclamped"/> between <see cref="range"/>'s <see cref="Vector2.x"/> and <see cref="Vector2.y"/>
        /// </summary>
        /// <param name="range"></param>
        /// <param name="lerpAmount"></param>
        /// <returns></returns>
        public static float LerpUnclamped(this Vector2 range, float lerpAmount) {
            return Mathf.LerpUnclamped(range.x, range.y, lerpAmount);
        }

        /// <summary>
        /// Returns <see cref="Vector2.y"/> - <see cref="Vector2.x"/>.
        /// </summary>
        /// <remarks>
        /// Alternate names from this might be:
        /// <ul>
        /// <li>Difference</li>
        /// <li>Distance</li>
        /// </ul>
        /// </remarks>
        /// <param name="range"></param>
        /// <returns></returns>
        public static float Delta(this Vector2 range) {
            return range.y - range.x;
        }

        /**
         * <inheritdoc cref="Delta(UnityEngine.Vector2)"/>
         */
        public static int Delta(this Vector2Int range) {
            return range.y - range.x;
        }

        /// <summary>
        /// Essentially an <see cref="Mathf.InverseLerp"/> equivalent to <see cref="Mathf.LerpUnclamped"/>.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float GetPortion(this Vector2 range, float point) {
            return (point - range.Delta()) / range.Delta();
        }

        /// <summary>
        /// Performs a <see cref="Mathf.InverseLerp"/> between <see cref="range"/>'s <see cref="Vector2.x"/> and <see cref="Vector2.y"/>.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float InverseLerp(this Vector2 range, float point) {
            return Mathf.InverseLerp(range.x, range.y, point);
            return range.GetPortion(point);
        }
    }
}