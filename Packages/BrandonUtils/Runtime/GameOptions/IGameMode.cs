namespace Packages.BrandonUtils.Runtime.GameOptions {
    public interface IGameMode {
        /// <summary>
        /// This should return an array containing all of the <see cref="GameOption"/>s specifically declared within the implementer.
        /// </summary>
        GameOption[] GameOptions { get; }
    }
}