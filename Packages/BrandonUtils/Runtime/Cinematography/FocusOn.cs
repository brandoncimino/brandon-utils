namespace BrandonUtils.Cinematography {
    public enum FocusOn {
        /// <summary>
        /// Directs the <see cref="DollyGrip"/> to look at the <see cref="DollyGrip.Subject_World"/>.
        /// </summary>
        Subject,
        /// <summary>
        /// Directs the <see cref="DollyGrip"/> to look at the <a href="https://en.wiktionary.org/wiki/middle_distance">middle distance</a>
        /// between their <see cref="DollyGrip.DollyMark_World"/> and the <see cref="DollyGrip.Subject_World"/>
        /// </summary>
        MiddleDistance,
        /// <summary>
        /// The <see cref="DollyGrip"/> won't actively look anywhere, essentially bypassing <see cref="DollyGrip.PointCamera"/>.
        /// </summary>
        Nothing
    }
}
