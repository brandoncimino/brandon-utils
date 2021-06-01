namespace BrandonUtils.Cinematography {
    public enum FocusOn {
        /// <summary>
        /// Refers directly to the <see cref="DollyCrewman.Subject"/>.
        /// </summary>
        Subject,
        /// <summary>
        /// Refers to the <a href="https://en.wiktionary.org/wiki/middle_distance">middle distance</a>, which is usually
        /// between the <see cref="DollyCrewman.Origin"/> and <see cref="DollyCrewman.Subject"/>.
        /// </summary>
        MiddleDistance,
        /// <summary>
        /// The <see cref="DollyCrewman"/> won't actively look anywhere.
        /// </summary>
        Nothing
    }
}
