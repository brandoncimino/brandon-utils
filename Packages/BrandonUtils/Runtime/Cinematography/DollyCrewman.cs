using UnityEngine;

namespace BrandonUtils.Cinematography {
    public abstract class DollyCrewman : MonoBehaviour {
        /// <summary>
        /// The <a href="https://www.creativelive.com/photography-guides/photography-subjects">subject</a> being filmed.
        /// </summary>
        public Transform Subject;
        public Transform Origin;
    }
}
