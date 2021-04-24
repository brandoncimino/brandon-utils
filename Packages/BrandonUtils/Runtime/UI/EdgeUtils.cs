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
            return edge switch {
                RectTransform.Edge.Left   => RectTransform.Axis.Horizontal,
                RectTransform.Edge.Right  => RectTransform.Axis.Horizontal,
                RectTransform.Edge.Top    => RectTransform.Axis.Vertical,
                RectTransform.Edge.Bottom => RectTransform.Axis.Vertical,
                _                         => throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge)
            };
        }

        /// <summary>
        /// Returns the <see cref="RectTransform.Edge"/> opposite of <paramref name="edge"/>, i.e. <see cref="RectTransform.Edge.Bottom"/> -> <see cref="RectTransform.Edge.Top"/>.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static RectTransform.Edge Inverse(this RectTransform.Edge edge) {
            return edge switch {
                RectTransform.Edge.Left   => RectTransform.Edge.Right,
                RectTransform.Edge.Right  => RectTransform.Edge.Left,
                RectTransform.Edge.Top    => RectTransform.Edge.Bottom,
                RectTransform.Edge.Bottom => RectTransform.Edge.Top,
                _                         => throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge)
            };
        }

        /// <summary>
        /// Returns the "<see cref="Math.Sign(decimal)"/>" of <paramref name="edge"/> along its <see cref="Axis"/>, i.e. <see cref="RectTransform.Edge.Left"/> -> -1; <see cref="RectTransform.Edge.Right"/> -> 1.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static int Direction(this RectTransform.Edge edge) {
            return edge switch {
                RectTransform.Edge.Left   => -1,
                RectTransform.Edge.Right  => 1,
                RectTransform.Edge.Top    => 1,
                RectTransform.Edge.Bottom => -1,
                _                         => throw EnumUtils.InvalidEnumArgumentException(nameof(edge), edge)
            };
        }
    }
}
