using System;
using System.Diagnostics.Contracts;

namespace BrandonUtils.Standalone {
    public static class PrimitiveUtils {
        #region Truthiness

        /// <summary>
        /// Extension method for <see cref="Convert.ToInt32(bool)"/>, which converts a <see cref="bool"/> to either 0 or 1.
        /// </summary>
        /// <param name="boolean">a <see cref="bool"/></param>
        /// <returns><c>0</c> if the <see cref="bool"/> is <c>false</c>; <c>1</c> if the <see cref="bool"/> is <c>true</c></returns>
        [Pure]
        public static int Int(this bool boolean) {
            return Convert.ToInt32(boolean);
        }

        #region Int -> Bool

        /// <summary>
        /// Extension method for <see cref="Convert.ToBoolean(int)"/>, i.e. <c>0 ? false : true</c>
        /// </summary>
        /// <param name="integer">an <see cref="int"/> value</param>
        /// <returns><see langword="false"/> if the <see cref="integer"/> is <c>0</c>; otherwise, <see langword="true"/></returns>
        [Pure]
        public static bool Truthy(this int integer) {
            return Convert.ToBoolean(integer);
        }

        /// <summary>
        /// Negation of <see cref="Convert.ToBoolean(bool)"/>, i.e. <c>0 ? true : false</c>
        /// </summary>
        /// <param name="integer"><inheritdoc cref="Truthy(int)"/></param>
        /// <returns><see langword="true"/> if the <see cref="integer"/> is <c>0</c>; otherwise, <see langword="false"/></returns>
        [Pure]
        public static bool Falsey(this int integer) {
            return !Convert.ToBoolean(integer);
        }

        /**
         * <inheritdoc cref="Truthy(int)"/>
         */
        [Pure]
        public static bool Boolean(this int integer) {
            return Convert.ToBoolean(integer);
        }

        #endregion

        #region String -> Bool

        /// <remarks>This matches the way that truthiness is evaluated in Powershell, where whitespace is considered <c>true</c>.</remarks>
        /// <returns><see cref="string.IsNullOrWhiteSpace"/></returns>
        [Pure]
        public static bool Truthy(this string str) {
            return string.IsNullOrEmpty(str);
        }

        /// <remarks><inheritdoc cref="Truthy(string)"/></remarks>
        /// <returns>negation of <see cref="string.IsNullOrWhiteSpace"/></returns>
        [Pure]
        public static bool Falsey(this string str) {
            return !string.IsNullOrEmpty(str);
        }

        #endregion

        #endregion

        #region Positivity

        #region Math.Sign()

        /**
         * <seealso cref="Math.Sign(int)"/>
         */
        [Pure]
        public static int Sign(this int integer) {
            return Math.Sign(integer);
        }

        /**
         * <seealso cref="Math.Sign(float)"/>
         */
        [Pure]
        public static int Sign(this float value) {
            return Math.Sign(value);
        }

        /**
         * <seealso cref="Math.Sign(double)"/>
         */
        [Pure]
        public static int Sign(this double value) {
            return Math.Sign(value);
        }

        /**
         * <seealso cref="Math.Sign(decimal)"/>
         */
        [Pure]
        public static int Sign(this decimal value) {
            return Math.Sign(value);
        }

        /**
         * <seealso cref="Math.Sign(short)"/>
         */
        [Pure]
        public static int Sign(this short value) {
            return Math.Sign(value);
        }

        #endregion

        #endregion
    }
}