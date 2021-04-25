using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Exceptions;

using UnityEngine;

namespace BrandonUtils.Spatial {
    public static class TransformUtils {
        public static Vector3 GetFace(this Transform self, Cube.Face face) {
            // Rider suggests storing these values rather than repeatedly accessing them. I'm pretty sure they're only accessed once, but whatever.
            var forward = self.forward;
            var right   = self.right;
            var up      = self.up;
            return face switch {
                Cube.Face.Forward  => forward,
                Cube.Face.Backward => -forward,
                Cube.Face.Left     => -right,
                Cube.Face.Right    => right,
                Cube.Face.Up       => up,
                Cube.Face.Down     => -up,
                _                  => throw EnumUtils.InvalidEnumArgumentException(nameof(face), face)
            };
        }

        /// <summary>
        /// Rotates <paramref name="self"/> so that:
        /// <ul>
        /// <li><paramref name="faceToPoint"/> ↬ <paramref name="target_world"/></li>
        /// <li><paramref name="upwardFace"/> ↬ <paramref name="up_world"/></li>
        /// </ul>
        /// </summary>
        /// <remarks>
        /// Relies on <see cref="Quaternion.FromToRotation"/>, so this <i>should</i> avoid <a href="https://en.wikipedia.org/wiki/Gimbal_lock#In_applied_mathematics">gimbal lock</a>.
        /// </remarks>
        /// <param name="self">the <see cref="Transform"/>to be rotated</param>
        /// <param name="faceToPoint">the <see cref="Cube.Face"/> that will point towards <paramref name="target_world"/>.<br/>
        /// Corresponds to the <c>target</c> in <see cref="Transform.LookAt(UnityEngine.Transform,UnityEngine.Vector3)"/></param>
        /// <param name="target_world"></param>
        /// <param name="upwardFace"></param>
        /// <param name="up_world"></param>
        /// <returns><paramref name="self"/>, for method chaining</returns>
        public static Transform PointFaceAt(
            this Transform self,
            Vector3 target_world,
            Cube.Face faceToPoint = Cube.Face.Forward,
            Cube.Face upwardFace = Cube.Face.Up,
            Vector3? up_world = default
        ) {
            if (faceToPoint == upwardFace) {
                throw new TransDimensionalException($"Both {nameof(faceToPoint)} and {nameof(upwardFace)} where {faceToPoint}! You cannot point the same face at {nameof(target_world)} _and_ {nameof(up_world)}! ");
            }

            var upQuat = Quaternion.FromToRotation(
                faceToPoint.NormalizedVector(),
                target_world - self.position
            );

            var faceQuat = Quaternion.FromToRotation(
                upwardFace.NormalizedVector(),
                up_world.GetValueOrDefault(Vector3.up)
            );

            self.rotation = upQuat * faceQuat;

            return self;
        }

        public static Transform LookAt(this Transform self, Vector3 target_world, Cube.Face upwardFace) {
            return self.PointFaceAt(target_world, Cube.Face.Forward, upwardFace);
        }
    }
}