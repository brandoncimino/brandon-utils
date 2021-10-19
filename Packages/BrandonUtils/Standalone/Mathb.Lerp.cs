// ReSharper disable UseDeconstructionOnParameter

namespace BrandonUtils.Standalone {
    public static partial class Mathb {
        public static float Lerp(float start, float finish, float lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var diff     = finish - start;
                var lerpDiff = diff * lerpAmount;
                return start + lerpDiff;
            }
        }

        public static double Lerp(double start, double finish, double lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var diff     = finish - start;
                var lerpDiff = diff * lerpAmount;
                return start + lerpDiff;
            }
        }

        public static decimal Lerp(decimal start, decimal finish, decimal lerpAmount) {
            if (lerpAmount <= 0) {
                return start;
            }
            else if (lerpAmount >= 1) {
                return finish;
            }
            else {
                var diff     = finish - start;
                var lerpDiff = diff * lerpAmount;
                return start + lerpDiff;
            }
        }

        public static float   Lerp(this (float start, float finish)     range, float   lerpAmount) => Lerp(range.start, range.finish, lerpAmount);
        public static double  Lerp(this (double start, double finish)   range, double  lerpAmount) => Lerp(range.start, range.finish, lerpAmount);
        public static decimal Lerp(this (decimal start, decimal finish) range, decimal lerpAmount) => Lerp(range.start, range.finish, lerpAmount);
    }
}