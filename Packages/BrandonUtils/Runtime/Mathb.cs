using BrandonUtils.Vectors;

using UnityEngine;

namespace BrandonUtils {
    /// <summary>
    /// Contains cute extension methods for primitive types, allowing things like <c>5.4f.Clamp01()</c>
    /// </summary>
    /// <remarks>
    /// GET IT! <see cref="Mathb"/>! Like <see cref="Mathf"/>!
    /// </remarks>
    public static class Mathb {
        /**
         * <inheritdoc cref="Mathf.Clamp(float,float,float)"/>
         */
        public static float Clamp(this float value, float min, float max) {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Calls <see cref="Mathf.Clamp(float,float,float)"/> using <see cref="range"/>'s <see cref="Vector2Utils.Min(UnityEngine.Vector2)"/> and <see cref="Vector2Utils.Max(UnityEngine.Vector2)"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static float Clamp(this float value, Vector2 range) {
            return Mathf.Clamp(value, range.Min(), range.Max());
        }

        /**
         * <inheritdoc cref="Mathf.Clamp01"/>
         */
        public static float Clamp01(this float value) {
            return Mathf.Clamp01(value);
        }

        /**
         * <inheritdoc cref="Mathf.Clamp(int, int, int)"/>
         */
        public static int Clamp(this int value, int min, int max) {
            return Mathf.Clamp(value, min, max);
        }

        /**
         * Calls <see cref="Mathf.Clamp(int,int,int)"/> using <see cref="range"/>'s <see cref="Vector2Utils.Min(UnityEngine.Vector2)"/> and <see cref="Vector2Utils.Max(UnityEngine.Vector2)"/>.
         */
        public static int Clamp(this int value, Vector2Int range) {
            return Mathf.Clamp(value, range.Min(), range.Max());
        }
    }
}