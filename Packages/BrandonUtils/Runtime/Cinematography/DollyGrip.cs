using BrandonUtils.Spatial;

using UnityEngine;

namespace BrandonUtils.Cinematography {
    public class DollyGrip : DollyCrewman {
        [Range(0, 1)]
        public float DollyLerp_X;
        [Range(0, 1)]
        public float DollyLerp_Y;
        [Range(0, 1)]
        public float DollyLerp_Z;

        private Vector3 DollyLerp => new Vector3(DollyLerp_X, DollyLerp_Y, DollyLerp_Z);

        private void MoveDolly() {
            var mark_world = Origin.LocalVerp(Subject.position, DollyLerp);
            transform.position = mark_world;
        }

        private void Update() {
            MoveDolly();
        }
    }
}
