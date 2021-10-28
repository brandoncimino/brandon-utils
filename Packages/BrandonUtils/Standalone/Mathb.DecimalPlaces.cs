using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        [Pure] public static double ShiftDecimal(this double value, int decimalPlaces) => value == 0 ? 0 : value * Math.Pow(10, decimalPlaces);
        [Pure] public static double ShiftDecimal(this int    value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);
        [Pure] public static double ShiftDecimal(this uint   value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);
        [Pure] public static double ShiftDecimal(this short  value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);
        [Pure] public static double ShiftDecimal(this ushort value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);
        [Pure] public static double ShiftDecimal(this float  value, int decimalPlaces) => ShiftDecimal(value.ToDouble(), decimalPlaces);
    }
}