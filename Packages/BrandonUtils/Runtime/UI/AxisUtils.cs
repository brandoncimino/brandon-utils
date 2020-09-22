using System;

using UnityEngine;

namespace Packages.BrandonUtils.Runtime.UI {
    public static class AxisUtils {
        public static int CoordinateIndex(this RectTransform.Axis axis) {
            switch (axis) {
                case RectTransform.Axis.Horizontal:
                    return 0;
                case RectTransform.Axis.Vertical:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static RectTransform.Axis Invert(this RectTransform.Axis axis) {
            switch (axis) {
                case RectTransform.Axis.Horizontal:
                    return RectTransform.Axis.Vertical;
                case RectTransform.Axis.Vertical:
                    return RectTransform.Axis.Horizontal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }
    }
}