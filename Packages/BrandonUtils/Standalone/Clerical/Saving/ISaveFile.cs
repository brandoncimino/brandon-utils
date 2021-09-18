namespace BrandonUtils.Standalone.Clerical.Saving {
    public interface ISaveFile : IHasFileInfo {
        public bool Save();
        public bool Load();
    }
}