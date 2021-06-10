using BrandonUtils.Vectors;

using UnityEngine;

namespace BrandonUtils.Mathb {
    public static class SugarMath {
        public static float Clamp(this float f, float min, float max) {
            return Mathf.Clamp(f, min, max);
        }

        public static float Clamp(this float f, Vector2 range) {
            return Mathf.Clamp(f, range.Min(), range.Max());
        }

        public static float Clamp01(this float f) {
            return Mathf.Clamp01(f);
        }

        public static int Clamp(this int i, int min, int max) {
            return Mathf.Clamp(i, min, max);
        }

        public static int Clamp(this int i, Vector2Int range) {
            return Mathf.Clamp(i, range.Min(), range.Max());
        }
    }
}