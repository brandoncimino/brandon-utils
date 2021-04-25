using BrandonUtils.Vectors;

using UnityEngine;

namespace BrandonUtils.Cinematography {
    public class DollyGrip : MonoBehaviour, IDollyCrew {
        public Transform Subject { get; set; }
        public Transform Origin  { get; set; }

        [Range(0, 1)]
        public float DollyLerp_X;
        [Range(0, 1)]
        public float DollyLerp_Y;
        [Range(0, 1)]
        public float DollyLerp_Z;

        private Vector3 DollyLerp => new Vector3(DollyLerp_X, DollyLerp_Y, DollyLerp_Z);

        public void MoveDolly() {
            var subject_local = Origin.transform.TransformPoint(Subject.position);
            var mark_local    = Vector3.zero.Verp(subject_local, DollyLerp);
            transform.position = mark_local;
        }

        private void Update() {
            MoveDolly();
        }
    }
}
