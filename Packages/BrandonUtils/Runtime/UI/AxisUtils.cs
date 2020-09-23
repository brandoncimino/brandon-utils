using System;

using Packages.BrandonUtils.Runtime.Enums;

using UnityEngine;

namespace Packages.BrandonUtils.Runtime.UI {
    /// <summary>
    /// Utilities for managing the <see cref="RectTransform.Axis"/> enum, such as converting between <see cref="RectTransform.Axis"/> and <see cref="RectTransform.Edge"/>.
    /// </summary>
    /// <remarks>
    /// It is important to remember that there are (as of 9/22/2020) only <b><i>two</i></b> <see cref="RectTransform.Axis"/> values - <see cref="RectTransform.Axis.Horizontal"/> and <see cref="RectTransform.Axis.Vertical"/> - and there is no <see cref="RectTransform.Axis"/> value corresponding to <see cref="Vector3.z"/>.
    /// <p/>
    /// We may consider creating an "Extension" of the <see cref="RectTransform.Axis"/> enum <i>(can <see cref="Enum"/>s inherit each other...?)</i> that adds in an additional "<b>Depth</b>" axis, though this adds an additional enum to convert between which may just increase confusion too much.
    /// </remarks>
    /// <seealso cref="RectTransform.Axis"/>
    /// <seealso cref="EdgeUtils"/>
    public static class AxisUtils {
        /// <summary>
        /// Converts an <see cref="RectTransform.Axis"/> into an index for use with a <see cref="Vector2"/> (or <see cref="Vector3">3</see>, or <see cref="Vector4">4</see>)'s <see cref="Vector2.this"/> accessor.
        /// </summary>
        /// <remarks>
        /// While it is true that the <see cref="RectTransform.Axis"/> <see cref="Enum"/>'s values can be explicitly converted to the correct integers (i.e., <see cref="RectTransform.Axis.Horizontal"/> is ordinal 1), this is more "defensive" (as in, unlikely to break if Unity introduces a fifth dimension), and this also makes the programmer's intent explicit (as opposed to vaguely casting an <see cref="Enum"/> to an int)
        /// </remarks>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int CoordinateIndex(this RectTransform.Axis axis) {
            switch (axis) {
                case RectTransform.Axis.Horizontal:
                    return 0;
                case RectTransform.Axis.Vertical:
                    return 1;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(axis), axis);
            }
        }

        /// <summary>
        /// "Inverts" <paramref name="axis"/>, returning <see cref="RectTransform.Axis.Horizontal"/> for <see cref="RectTransform.Axis.Vertical"/>, etc.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static RectTransform.Axis Invert(this RectTransform.Axis axis) {
            switch (axis) {
                case RectTransform.Axis.Horizontal:
                    return RectTransform.Axis.Vertical;
                case RectTransform.Axis.Vertical:
                    return RectTransform.Axis.Horizontal;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(axis), axis);
            }
        }
    }
}