using System;

using BrandonUtils.Timing;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Saving {
    /**
     * An interface for <see cref="SaveData{T}"/>.
     *
     * Specifies the generic behaviors of save data - saving, loading, checking the times of that and stuff - but not any of the actual data that the save data should contain.
     */
    public interface ISaveData {
        [NotNull]
        [JsonProperty]
        string Nickname { get; set; }
        [JsonProperty]
        DateTime LastSaveTime { get; set; }
        [JsonProperty]
        DateTime LastSaveTime_Exact { get; set; }
        [JsonIgnore]
        string[] AllSaveFilePaths { get; }
        [JsonIgnore]
        string LatestSaveFilePath { get; }
        [JsonIgnore]
        string OldestSaveFilePath { get; }
        [JsonIgnore]
        bool Exists { get; }

        /// <summary>
        /// The time that this <see cref="SaveData{T}"/> was loaded.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="FrameTime.Now"/> when the data is initialized, <see cref="Load"/>-ed, or <see cref="Reload"/>-ed.
        /// </remarks>
        [JsonIgnore]
        DateTime LastLoadTime { get; set; }

        /// <summary>
        ///     Calls the static <see cref="SaveData{T}.Save(BrandonUtils.Saving.SaveData{T},string,bool)" /> with this <see cref="SaveData{T}" />'s <see cref="SaveData{T}.Nickname" />.
        ///     <br />
        /// </summary>
        /// <param name="useReSaveDelay">If <c>true</c>, check if <see cref="SaveData.ReSaveDelay" /> has elapsed since <see cref="SaveData{T}.LastSaveTime" />.</param>
        /// <exception cref="ReSaveDelayException">If <paramref name="useReSaveDelay" /> is <c>true</c> and <see cref="SaveData.ReSaveDelay" /> hasn't elapsed since <see cref="SaveData{T}.LastSaveTime" />.</exception>
        void Save(bool useReSaveDelay = true);

        /// <summary>
        ///     The canon way to convert a <see cref="SaveData{T}"/> to a json.
        /// </summary>
        /// <returns></returns>
        string ToJson();
    }
}