using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

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
         * <seealso cref="Math.Sign(short)"/>
         */
        [Pure]
        public static int Sign(this short value) {
            return Math.Sign(value);
        }

        /**
         * <seealso cref="Math.Sign(int)"/>
         */
        [Pure]
        public static int Sign(this int integer) {
            return Math.Sign(integer);
        }

        /**
         * <seealso cref="Math.Sign(long)"/>
         */
        [Pure]
        public static int Sign(this long value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(float)"/>
         */
        [Pure]
        public static int Sign(this float value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(double)"/>
         */
        [Pure]
        public static int Sign(this double value) => Math.Sign(value);

        /**
         * <seealso cref="Math.Sign(decimal)"/>
         */
        [Pure]
        public static int Sign(this decimal value) {
            return Math.Sign(value);
        }

        #endregion

        #region IsPositive

        public static bool IsPositive(this short value) => value >= 0;
        public static bool IsPositive(this int value) => value >= 0;
        public static bool IsPositive(this long value) => value >= 0;
        public static bool IsPositive(this float value) => value >= 0;
        public static bool IsPositive(this double value) => value >= 0;
        public static bool IsPositive(this decimal value) => value >= 0;

        #endregion

        #region IsStrictlyPositive

        public static bool IsStrictlyPositive(int value) => value > 0;
        public static bool IsStrictlyPositive(short value) => value > 0;
        public static bool IsStrictlyPositive(long value) => value > 0;
        public static bool IsStrictlyPositive(float value) => value > 0;
        public static bool IsStrictlyPositive(double value) => value > 0;
        public static bool IsStrictlyPositive(decimal value) => value > 0;

        #endregion

        #region IsNegative

        public static bool IsNegative(this int value) => value < 0;
        public static bool IsNegative(this long value) => value < 0;
        public static bool IsNegative(this float value) => value < 0;
        public static bool IsNegative(this double value) => value < 0;
        public static bool IsNegative(this short value) => value < 0;
        public static bool IsNegative(this decimal value) => value < 0;

        #endregion

        #endregion

        #region Type-checking

        /// <summary>
        /// See <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">Integral Numeric Types</a>
        /// </summary>
        private static readonly ReadOnlyCollection<Type> IntegralTypes = Array.AsReadOnly(
            new[] {
                typeof(byte),
                typeof(sbyte),
                typeof(ushort),
                typeof(short),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
            }
        );

        /// <summary>
        /// See <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">Floating-Point Numeric Types</a>
        /// </summary>
        private static readonly ReadOnlyCollection<Type> FloatingPointTypes = Array.AsReadOnly(
            new[] {
                typeof(float),
                typeof(double),
                typeof(decimal)
            }
        );

        /// <summary>
        /// Both <see cref="IntegralTypes"/> and <see cref="FloatingPointTypes"/>.
        /// </summary>
        public static readonly ReadOnlyCollection<Type> NumericTypes = FloatingPointTypes.Union(IntegralTypes).ToList().AsReadOnly();
        /// <summary>
        /// Special <see cref="NumericTypes"/> that are automatically cast to <see cref="Int32"/> when used in <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/arithmetic-operators">arithmetic</a>.
        /// </summary>
        public static readonly ReadOnlyCollection<Type> PseudoIntTypes = Array.AsReadOnly(new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort) });

        /// <summary>
        /// Returns whether or not the given <see cref="value"/> is one of the <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types">integral numeric types</a> or <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types">floating-point numeric types</a>.
        /// </summary>
        /// <param name="value">some random junk</param>
        /// <returns>true if the value is of a numeric type</returns>
        public static bool IsNumber(this object value) {
            return NumericTypes.Contains(value.GetType());
        }

        #endregion
    }
}