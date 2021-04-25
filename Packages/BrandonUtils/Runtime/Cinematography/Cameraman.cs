using BrandonUtils.Standalone.Enums;
using BrandonUtils.Vectors;

using UnityEngine;

namespace BrandonUtils.Cinematography {
    [RequireComponent(typeof(Camera))]
    public class Cameraman : MonoBehaviour, IDollyCrew {
        public Transform Subject { get; set; }
        public Transform Origin  { get; set; }

        /// <summary>
        /// The <see cref="FocusOn"/> style that the <see cref="DollyGrip"/> uses, which determines how they will point their camera.
        /// </summary>
        public FocusOn LookStyle = FocusOn.Nothing;

        [Range(0, 1)]
        public float MiddleDistanceLerp_X;
        [Range(0, 1)]
        public float MiddleDistanceLerp_Y;
        [Range(0, 1)]
        public float MiddleDistanceLerp_Z;

        private Vector3 MiddleDistanceLerp => new Vector3(MiddleDistanceLerp_X, MiddleDistanceLerp_Y, MiddleDistanceLerp_Z);

        public void Update() {
            PointCamera();
        }

        public void PointCamera() {
            switch (LookStyle) {
                case FocusOn.Nothing:
                    // no-op
                    return;
                case FocusOn.Subject:
                    transform.LookAt(Subject, Origin.up);
                    return;
                case FocusOn.MiddleDistance:
                    var subject_local = Origin.TransformPoint(Subject.position);
                    var middist_local = Vector3.zero.Verp(subject_local, MiddleDistanceLerp);
                    var middist_world = Origin.TransformPoint(middist_local);
                    transform.LookAt(middist_world, Origin.up);
                    return;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(LookStyle), LookStyle);
            }
        }
    }
}
