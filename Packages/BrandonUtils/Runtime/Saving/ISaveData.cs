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
        string Nickname { get; }
        [JsonProperty]
        DateTime? LastSaveTime { get; }
        [JsonProperty]
        DateTime? LastSaveTime_Exact { get; }
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
        ///
        /// If the <see cref="ISaveData"/> has <b>never</b> been loaded, then <see cref="Nullable{T}.HasValue"/> should be false.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="FrameTime.Now"/> when the data is <see cref="SaveData{T}.Load"/>-ed or <see cref="ISaveData{T}.Reload"/>-ed.
        /// <br/>
        /// Set to null when the data is initialized or <see cref="ISaveData{T}.Reset"/>.
        /// </remarks>
        [JsonIgnore]
        DateTime? LastLoadTime { get; }

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

    /// <summary>
    /// Generic version of <see cref="ISaveData"/>, which primarily contains methods that return implementor types such as <see cref="Reload"/>.
    /// </summary>
    /// <typeparam name="T">the actual type that inherits from <see cref="ISaveData{T}"/></typeparam>
    public interface ISaveData<out T> : ISaveData where T : ISaveData<T> {
        /// <summary>
        /// Loads the most recent version of the save file.
        /// </summary>
        /// <remarks>
        /// This utilizes <see cref="JsonConvert.PopulateObject(string,object)"/> rather than <see cref="JsonConvert.DeserializeObject(string)"/>.
        /// <p/>
        /// <see cref="JsonConvert.DeserializeObject(string)"/> has wrappers that throw <see cref="SaveDataException"/>s - <see cref="DeserializeByPath"/>, etc. - so I considered creating analogous methods for <see cref="JsonConvert.PopulateObject(string,object)"/>, e.g. "PopulateByPath".
        /// <p/>
        /// However, the intricacies of <see cref="JsonConvert.PopulateObject(string,object)"/> - for example, why is it able to populate the <c>target</c> object without using a <see langword="ref"/> parameter - didn't seem practical to tease out.
        /// </remarks>
        /// <returns></returns>
        T Reload();

        /// <summary>
        /// Resets all of the values of the <see cref="ISaveData"/> as though it were a new instance - <b>except</b> for <see cref="Nickname"/>
        /// </summary>
        /// <remarks>
        /// This was made to handle weirdness that was arising through <see cref="JsonConvert.PopulateObject(string,object)"/> due to circular references in child objects (specifically, this happened in Fortune Fountain G)
        /// </remarks>
        /// <returns><see langword="this"/></returns>
        T Reset();
    }
}