using System;

using BrandonUtils.Vectors;

using UnityEngine;

namespace BrandonUtils {
    /// <summary>
    /// Contains cute extension methods for primitive types, allowing things like <c>5.4f.Clamp01()</c>
    ///
    /// TODO: The reference to UnityEngine should be removed, and this should be moved into <see cref="Standalone"/>.
    /// </summary>
    /// <remarks>
    /// GET IT! <see cref="Mathb"/>! Like <see cref="Mathf"/>!
    /// </remarks>
    public static class Mathb {
        #region Clamp

        /**
         * <inheritdoc cref="Mathf.Clamp(float,float,float)"/>
         */
        public static float Clamp(this float value, float min, float max) {
            if (value <= min) {
                return min;
            }
            else if (value >= max) {
                return max;
            }
            else {
                return value;
            }
        }

        /// <summary>
        /// Calls <see cref="Mathf.Clamp(float,float,float)"/> using <see cref="range"/>'s <see cref="Vector2Utils.Min(UnityEngine.Vector2)"/> and <see cref="Vector2Utils.Max(UnityEngine.Vector2)"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static float Clamp(this float value, Vector2 range) {
            return Clamp(value, range.Min(), range.Max());
        }

        /**
         * <inheritdoc cref="Mathf.Clamp01"/>
         */
        public static float Clamp01(this float value) {
            return value.Clamp(0, 1);
        }

        /**
         * <inheritdoc cref="Mathf.Clamp(int, int, int)"/>
         */
        public static int Clamp(this int value, int min, int max) {
            if (value <= min) {
                return min;
            }
            else if (value >= max) {
                return max;
            }
            else {
                return value;
            }
        }

        /**
         * Calls <see cref="Mathf.Clamp(int,int,int)"/> using <see cref="range"/>'s <see cref="Vector2Utils.Min(UnityEngine.Vector2)"/> and <see cref="Vector2Utils.Max(UnityEngine.Vector2)"/>.
         */
        public static int Clamp(this int value, Vector2Int range) {
            return Clamp(value, range.Min(), range.Max());
        }

        #endregion

        #region Min

        #region Min (two inputs)

        public static short Min(this short value, short other) => Math.Min(value,       other);
        public static int Min(this int value, int other) => Math.Min(value,             other);
        public static long Min(this long value, long other) => Math.Min(value,          other);
        public static float Min(this float value, float other) => Math.Min(value,       other);
        public static double Min(this double value, double other) => Math.Min(value,    other);
        public static decimal Min(this decimal value, decimal other) => Math.Min(value, other);
        public static ushort Min(this ushort value, ushort other) => Math.Min(value,    other);
        public static uint Min(this uint value, uint other) => Math.Min(value,          other);
        public static ulong Min(this ulong value, ulong other) => Math.Min(value,       other);
        public static byte Min(this byte value, byte other) => Math.Min(value,          other);
        public static sbyte Min(this sbyte value, sbyte other) => Math.Min(value,       other);

        #endregion

        #endregion

        #region Max

        #region Max (two inputs)

        public static short Max(this short value, short other) => Math.Max(value,       other);
        public static int Max(this int value, int other) => Math.Max(value,             other);
        public static long Max(this long value, long other) => Math.Max(value,          other);
        public static float Max(this float value, float other) => Math.Max(value,       other);
        public static double Max(this double value, double other) => Math.Max(value,    other);
        public static decimal Max(this decimal value, decimal other) => Math.Max(value, other);
        public static ushort Max(this ushort value, ushort other) => Math.Max(value,    other);
        public static uint Max(this uint value, uint other) => Math.Max(value,          other);
        public static ulong Max(this ulong value, ulong other) => Math.Max(value,       other);
        public static byte Max(this byte value, byte other) => Math.Max(value,          other);
        public static sbyte Max(this sbyte value, sbyte other) => Math.Max(value,       other);

        #endregion

        #endregion
    }
}