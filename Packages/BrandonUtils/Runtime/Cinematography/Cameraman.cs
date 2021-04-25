using BrandonUtils.Spatial;
using BrandonUtils.Standalone.Enums;

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

        private Vector3 MiddleDistanceLerp => new Vector3(MiddleDistanceLerp_X, MiddleDistanceLerp_Y, 1);

        public void Update() {
            PointCamera();
        }

        private void PointCamera() {
            switch (LookStyle) {
                case FocusOn.Nothing:
                    // no-op
                    return;
                case FocusOn.Subject:
                    transform.LookAt(Subject, Origin.up);
                    return;
                case FocusOn.MiddleDistance:
                    transform.LookAt(TransformUtils.LocalVerp(Origin, Subject.position, MiddleDistanceLerp), Origin.up);
                    return;
                default:
                    throw EnumUtils.InvalidEnumArgumentException(nameof(LookStyle), LookStyle);
            }
        }
    }
}
