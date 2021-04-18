using System;
using System.ComponentModel;

using BrandonUtils.Standalone.Enums;

using UnityEngine;

namespace BrandonUtils.UI {
    public static class EdgeUtils {
        /// <summary>
        /// Gets the relevant <see cref="RectTransform.Axis"/> for <paramref name="edge"/>, i.e. <see cref="RectTransform.Edge.Bottom"/> -> <see cref="RectTransform.Axis.Vertical"/>.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static RectTransform.Axis Axis(this RectTransform.Edge edge) {
            switch (edge) {
                case RectTransform.Edge.Left:
                case RectTransform.Edge.Right:
                    return RectTransform.Axis.Horizontal;
                case RectTransform.Edge.Top:
                case RectTransform.Edge.Bottom:
                    return RectTransform.Axis.Vertical;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge);
            }
        }

        /// <summary>
        /// Returns the <see cref="RectTransform.Edge"/> opposite of <paramref name="edge"/>, i.e. <see cref="RectTransform.Edge.Bottom"/> -> <see cref="RectTransform.Edge.Top"/>.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static RectTransform.Edge Inverse(this RectTransform.Edge edge) {
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
                    throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge);
            }
        }

        /// <summary>
        /// Returns the "<see cref="Math.Sign(decimal)"/>" of <paramref name="edge"/> along its <see cref="Axis"/>, i.e. <see cref="RectTransform.Edge.Left"/> -> -1; <see cref="RectTransform.Edge.Right"/> -> 1.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
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
                    throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge);
            }
        }
    }
}