using System;

namespace BrandonUtils.Saving {
    /// <summary>
    /// A special <see cref="SaveDataException"/> thrown when a <see cref="SaveData{T}"/> is <see cref="SaveData{T}.Save(BrandonUtils.Saving.SaveData{T},string,bool)"/>-ed before <see cref="SaveData.ReSaveDelay"/> has elapsed.
    /// </summary>
    public class ReSaveDelayException : SaveDataException {
        public ReSaveDelayException(ISaveData saveData) : base(saveData) { }
        public ReSaveDelayException(ISaveData saveData, string message) : base(saveData, message) { }
        public ReSaveDelayException(ISaveData saveData, Exception innerException) : base(saveData, innerException) { }
        public ReSaveDelayException(Exception innerException) : base(innerException) { }
        public ReSaveDelayException(string message, Exception innerException) : base(message, innerException) { }
        public ReSaveDelayException(string message) : base(message) { }
        public ReSaveDelayException(ISaveData saveData, string message, Exception innerException) : base(saveData, message, innerException) { }
    }
}