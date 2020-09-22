using System;

using UnityEngine;

namespace Packages.BrandonUtils.Runtime.UI {
    public static class EdgeUtils {
        public static RectTransform.Axis Axis(this RectTransform.Edge edge) {
            switch (edge) {
                case RectTransform.Edge.Left:
                case RectTransform.Edge.Right:
                    return RectTransform.Axis.Horizontal;
                case RectTransform.Edge.Top:
                case RectTransform.Edge.Bottom:
                    return RectTransform.Axis.Vertical;
                default:
                    throw new ArgumentOutOfRangeException(nameof(edge), $"I have no idea what edge {edge} is!");
            }
        }

        public static RectTransform.Edge Invert(this RectTransform.Edge edge) {
            switch (edge) {
                case RectTransform.Edge.Left:
                    return RectTransform.Edge.Right;
                case RectTransform.Edge.Right:
                    return RectTransform.Edge.Left;
                case RectTransform.Edge.Top:
                    return RectTransform.Edge.Bottom;
                case RectTransform.Edge.Bottom:
                    return RectTransform.Edge.Top;
                default:
                    throw new ArgumentOutOfRangeException(nameof(edge), edge, $"What even is {edge}?!");
            }
        }

        public static int Direction(this RectTransform.Edge edge) {
            switch (edge) {
                case RectTransform.Edge.Left:
                    return -1;
                case RectTransform.Edge.Right:
                    return 1;
                case RectTransform.Edge.Top:
                    return 1;
                case RectTransform.Edge.Bottom:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(edge), edge, null);
            }
        }
    }
}