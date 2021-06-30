namespace BrandonUtils.Saving {
    /// <summary>
    ///     An example implementation of <see cref="SaveData{T}"/>.
    /// </summary>
    /// <remarks>
    ///     Intended only for use by unit tests.
    /// </remarks>
    public class SaveDataTestImpl : SaveData<SaveDataTestImpl> {
        public string Word;

        public string Word2 = "My Mom";

        public SaveDataTestImpl(string nickname) : base(nickname) { }
    }
}