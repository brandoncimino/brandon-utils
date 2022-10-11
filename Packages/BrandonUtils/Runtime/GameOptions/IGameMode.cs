namespace BrandonUtils.GameOptions {
    /// <summary>
    /// A collection of <see cref="GameOption"/>s.
    /// </summary>
    public interface IGameMode {
        /// <summary>
        /// This should return an array containing all of the <see cref="GameOption"/>s specifically declared within the implementer.
        /// </summary>
        GameOption[] GameOptions { get; }
    }
}