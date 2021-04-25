using UnityEngine;

namespace BrandonUtils.Cinematography {
    public interface IDollyCrew {
        /// <summary>
        /// The <a href="https://www.creativelive.com/photography-guides/photography-subjects">subject</a> being filmed.
        /// </summary>
        public Transform Subject { get; set; }
        public Transform Origin { get;  set; }
    }
}
