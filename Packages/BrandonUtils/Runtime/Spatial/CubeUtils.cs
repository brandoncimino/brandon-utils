using System.ComponentModel;

using BrandonUtils.Standalone.Enums;

using UnityEngine;

namespace BrandonUtils.Spatial {
    public static class CubeUtils {
        /// <summary>
        /// Returns the the corresponding <see cref="Vector3.normalized"/> <see cref="Vector3"/> that <paramref name="self"/> points to,
        /// e.g. <see cref="Cube.Face"/>.<see cref="Cube.Face.Forward"/>.<see cref="NormalizedVector"/> -> <see cref="Vector3"/>.<see cref="Vector3.forward"/>
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException">If an unknown </exception>
        public static Vector3 NormalizedVector(this Cube.Face self) {
            return self switch {
                Cube.Face.Forward  => Vector3.forward,
                Cube.Face.Backward => Vector3.back,
                Cube.Face.Left     => Vector3.left,
                Cube.Face.Right    => Vector3.right,
                Cube.Face.Up       => Vector3.up,
                Cube.Face.Down     => Vector3.down,
                _                  => throw EnumUtils.InvalidEnumArgumentException(nameof(self), self)
            };
        }
    }
}