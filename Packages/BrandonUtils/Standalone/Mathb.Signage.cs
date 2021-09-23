﻿using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        #region Signage

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

        public static bool IsPositive(this short   value) => value >= 0;
        public static bool IsPositive(this int     value) => value >= 0;
        public static bool IsPositive(this long    value) => value >= 0;
        public static bool IsPositive(this float   value) => value >= 0;
        public static bool IsPositive(this double  value) => value >= 0;
        public static bool IsPositive(this decimal value) => value >= 0;

        #endregion

        #region IsStrictlyPositive

        public static bool IsStrictlyPositive(int     value) => value > 0;
        public static bool IsStrictlyPositive(short   value) => value > 0;
        public static bool IsStrictlyPositive(long    value) => value > 0;
        public static bool IsStrictlyPositive(float   value) => value > 0;
        public static bool IsStrictlyPositive(double  value) => value > 0;
        public static bool IsStrictlyPositive(decimal value) => value > 0;

        #endregion

        #region IsNegative

        public static bool IsNegative(this int     value) => value < 0;
        public static bool IsNegative(this long    value) => value < 0;
        public static bool IsNegative(this float   value) => value < 0;
        public static bool IsNegative(this double  value) => value < 0;
        public static bool IsNegative(this short   value) => value < 0;
        public static bool IsNegative(this decimal value) => value < 0;

        #endregion

        #endregion
    }
}