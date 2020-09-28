using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Packages.BrandonUtils.Runtime.UI {
    public static class RectTransformUtils {
        public static RectTransform Sidle(
            this RectTransform me,
            RectTransform.Edge myEdge,
            RectTransform target,
            float padding = 0
        ) {
            var oldParent = me.parent;
            me.SetParent(target);
            var size = me.GetAxisSize(myEdge.Axis()) + padding;
            me.SetInsetAndSizeFromParentEdge(myEdge.Inverse(), -size, size);
            me.SetParent(oldParent);

            //post-conditions
            if (me.parent != oldParent) {
                throw new Exception("We didn't get back to our original parent!!");
            }

            return me;
        }

        public static RectTransform Align(this RectTransform me, RectTransform.Edge edge, float target) {
            var edgePosition = me.GetEdgePosition_AsFloat(edge);
            var offset       = target - edgePosition;

            var offsetVector = new Vector3 {[edge.Axis().CoordinateIndex()] = offset};

            me.position += offsetVector;

            return me;
        }

        public static RectTransform Align(this RectTransform me, RectTransform.Axis axis, float target) {
            var axisVisualCenterPosition = me.GetVisualCenter()[axis.CoordinateIndex()];
            var offset                   = target - axisVisualCenterPosition;

            var offsetVector = new Vector3 {[axis.CoordinateIndex()] = offset};
            me.position += offsetVector;

            return me;
        }

        public static RectTransform Center(this RectTransform me, RectTransform.Axis axis, float target) =>
            Align(me, axis, target);

        /// <summary>
        /// Returns the position of <paramref name="me"/> as though <paramref name="me"/> had a <see cref="RectTransform.pivot"/> of <see cref="Vector2.zero"/>.
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static Vector3 GetVisualCenter(this RectTransform me) {
            var left  = me.GetEdgePosition_AsFloat(RectTransform.Edge.Left);
            var right = me.GetEdgePosition_AsFloat(RectTransform.Edge.Right);
            var x     = (left + right) / 2;

            var top    = me.GetEdgePosition_AsFloat(RectTransform.Edge.Top);
            var bottom = me.GetEdgePosition_AsFloat(RectTransform.Edge.Bottom);
            var y      = (top + bottom) / 2;

            return new Vector3(x, y, 0);
        }

        public static void LineUp(ICollection<RectTransform> suspects, RectTransform.Edge direction, float padding = 0) {
            var leader    = suspects.First();
            var followers = suspects.Skip(0);
            LineUp(leader, followers, direction, padding);
        }

        public static void LineUp(
            RectTransform leader,
            IEnumerable<RectTransform> followers,
            RectTransform.Edge direction,
            float padding = 0
        ) {
            var previousInLine = leader;
            foreach (var nextInLine in followers) {
                nextInLine.Sidle(direction.Inverse(), previousInLine, padding);
                previousInLine = nextInLine;
            }
        }

        public static float GetAxisSize(this RectTransform me, RectTransform.Axis axis) {
            switch (axis) {
                case RectTransform.Axis.Horizontal:
                    return me.rect.width;
                case RectTransform.Axis.Vertical:
                    return me.rect.height;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, $"I have no idea what axis {axis} is!");
            }
        }

        public static float GetAxisPosition_AsFloat(this RectTransform me, RectTransform.Axis axis) {
            switch (axis) {
                case RectTransform.Axis.Horizontal:
                    return me.position.x;
                case RectTransform.Axis.Vertical:
                    return me.position.y;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static Vector2 GetAxisPosition_AsVector2(this RectTransform me, RectTransform.Axis axis) {
            switch (axis) {
                case RectTransform.Axis.Horizontal:
                    return Vector2.right * GetAxisPosition_AsFloat(me, axis);
                case RectTransform.Axis.Vertical:
                    return Vector2.up * GetAxisPosition_AsFloat(me, axis);
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }
        }

        public static float DistanceFromPivotToEdge(this RectTransform me, RectTransform.Edge edge) {
            switch (edge) {
                case RectTransform.Edge.Left:
                    return me.rect.width * me.pivot.x;
                case RectTransform.Edge.Right:
                    return me.rect.width * (1 - me.pivot.x);
                case RectTransform.Edge.Top:
                    return me.rect.height * (1 - me.pivot.y);
                case RectTransform.Edge.Bottom:
                    return me.rect.height * me.pivot.y;
                default:
                    throw new ArgumentOutOfRangeException(nameof(edge), edge, null);
            }
        }

        public static float GetEdgePosition_AsFloat(this RectTransform me, RectTransform.Edge edge) {
            var relevantPosition = me.position[edge.Axis().CoordinateIndex()];
            var distanceToEdge   = me.DistanceFromPivotToEdge(edge);
            var edgePosition     = relevantPosition + (distanceToEdge * edge.Direction());
            return edgePosition;
        }
    }
}