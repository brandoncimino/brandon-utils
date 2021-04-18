using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;

using UnityEngine;

namespace BrandonUtils {
    public static class PairUtils {
        public static Pair<float, float> ToPair(this Vector2 vector2) {
            return new Pair<float, float>(vector2.x, vector2.y);
        }

        public static Vector2 ToVector2(this Pair<float, float> pair) {
            return new Vector2(pair.X, pair.Y);
        }

        public static Dictionary<TX, TY> ToDictionary<TX, TY>(this IEnumerable<Pair<TX, TY>> pairCollection) {
            return pairCollection.ToDictionary(pair => pair.X, pair => pair.Y);
        }
    }
}