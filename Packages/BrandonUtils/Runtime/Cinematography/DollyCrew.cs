using UnityEngine;

namespace BrandonUtils.Cinematography {
    public class DollyCrew : MonoBehaviour {
        public Transform Subject;
        public DollyGrip DollyGrip;
        public Cameraman Cameraman;

        /**
         * TODO: Find a way to have the DollyCrew "Manage" the Cameraman and DollyGrip, the way that "LayoutGroups" work
         */
        public void Start() {
            DollyGrip         = GetComponentInChildren<DollyGrip>();
            DollyGrip.Origin  = transform;
            DollyGrip.Subject = Subject;

            Cameraman         = GetComponentInChildren<Cameraman>();
            Cameraman.Subject = Subject;
            Cameraman.Origin  = DollyGrip.transform;
        }
    }
}
