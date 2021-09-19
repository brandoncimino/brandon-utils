using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical.Saving {
    /// <summary>
    /// Joins together a <see cref="System.IO.FileInfo"/> with the <see cref="ISaveData"/> it contains.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [PublicAPI]
    public interface ISaveFile<TData> : IHasFileInfo where TData : ISaveData {
        [JsonProperty]
        [CanBeNull]
        public TData Data { get; }

        [JsonIgnore]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Internal-only setter for <see cref="TData"/>. Used by <see cref="SaveFileExtensions.Load{TFile,TData}"/>.
        /// </summary>
        /// <param name="data"></param>
        internal void SetDataInternal([CanBeNull] TData data);
    }
}