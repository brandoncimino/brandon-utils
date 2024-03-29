﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using BrandonUtils.Logging;
using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Timing;

using JetBrains.Annotations;

using Newtonsoft.Json;

using UnityEngine;

using static BrandonUtils.Logging.LogUtils;

// ReSharper disable StaticMemberInGenericType

[assembly: InternalsVisibleTo("BrandonUtils.Tests.Playmode")]

namespace BrandonUtils.Saving {
    /// <summary>
    /// Non-generic base class for <see cref="SaveData{T}"/>
    ///
    /// TODO: Look for things that were made public but are only used for testing, and switch them to `internal`
    /// </summary>
    [PublicAPI]
    public abstract class SaveData {
        [JsonIgnore]
        [UsedImplicitly]
        public static string SaveFolderName { get; } = nameof(SaveData);
        [JsonIgnore]
        public const string AutoSaveName = "AutoSave";
        [JsonIgnore]
        public const string SaveFileExtension = "sav";
        [JsonIgnore]
        public const int BackupSaveSlots = 10;

        /// <returns>A <b>new</b> instance of the default <see cref="CurrentJsonSerializerSettings"/></returns>
        public static JsonSerializerSettings GetDefaultSerializerSettings() => new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace, Formatting = Formatting.Indented, TypeNameHandling = TypeNameHandling.Auto, };

        /// <summary>
        /// The actual <see cref="JsonSerializerSettings"/> being used.
        ///
        /// Defaults to <see cref="GetDefaultSerializerSettings"/>, but can be freely modified.
        /// </summary>
        [JsonIgnore]
        public static JsonSerializerSettings CurrentJsonSerializerSettings { get; set; } = GetDefaultSerializerSettings();

        /// <summary>
        /// The required length of timestamps in save file names generated via <see cref="SaveData{T}.GetSaveFileNameWithDate"/>
        /// </summary>
        public const int TimeStampLength = 18;

        /// <summary>
        /// Timestamps will be serialized into file names using their <see cref="DateTime.Ticks" /> value, which will have 18 digits until 11/16/3169 09:46:40.
        /// </summary>
        /// <remarks>
        /// Update from Brandon on 8/8/2021: I don't understand why I did this instead of just using <c>\d+</c>
        /// Maybe it was so that the nickname pattern could be `.*`? That would make _some_ sense, I guess...it's still weird though and probably not necessary.
        /// OH! Now I remember! It was so that I could properly limit the save nickname length, because the timestamp would have a reliable length!
        /// </remarks>
        public static readonly string TimeStampPattern = $@"\d{{{TimeStampLength}}}";
        public static readonly string   SaveFilePattern = $@"(?<nickName>.*)_(?<date>{TimeStampPattern})";
        public static readonly TimeSpan ReSaveDelay     = TimeSpan.FromSeconds(1);
    }

    /// <summary>
    ///     A single "Save File", containing data
    /// </summary>
    /// <remarks>
    ///     <para>Implementations will serialize all <see cref="SerializeField" />s in both <see cref="SaveData{T}" /> and the inheritor, such as <see cref="SaveDataTestImpl" />.</para>
    ///     <para>Uses the <a href="https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern">Curiously Repeating Template Pattern</a>, where <typeparamref name="T" /> references the inheritor type, e.g. <see cref="SaveDataTestImpl" />.</para>
    /// </remarks>
    /// <typeparam name="T">The inheriting class, e.g. <see cref="SaveDataTestImpl" /></typeparam>
    /// <seealso cref="SaveDataTestImpl" />
    public abstract class SaveData<T> : SaveData, ISaveData<T> where T : SaveData<T> {
        [JsonIgnore]
        [UsedImplicitly]
        public static readonly string SaveTypeName = typeof(T).Name;

        [JsonIgnore]
        [UsedImplicitly]
        public static string SaveFolderPath => Path.Combine(
            Application.persistentDataPath,
            SaveFolderName,
            SaveTypeName
        );

        [JsonProperty]
        public string Nickname { get; private set; }

        [JsonProperty]
        public DateTime? LastSaveTime { get; protected set; }

        [JsonProperty]
        public DateTime? LastSaveTime_Exact { get; protected set; }

        [JsonIgnore]
        public string[] AllSaveFilePaths => GetAllSaveFilePaths(Nickname);

        [JsonIgnore]
        public string LatestSaveFilePath => GetLatestSaveFilePath(Nickname);

        [JsonIgnore]
        public string OldestSaveFilePath => GetOldestSaveFilePath(Nickname);

        [JsonIgnore]
        public bool Exists => SaveFileExists(Nickname);

        /// <summary>
        /// The time that this <see cref="SaveData{T}"/> was loaded.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="FrameTime.Now"/> when the data is initialized, <see cref="Load"/>-ed, or <see cref="Reload"/>-ed.
        /// </remarks>
        [JsonIgnore]
        public DateTime? LastLoadTime { get; set; }

        /**
         * Stores an "empty" instance of <typeparamref name="T"/> for use with <see cref="Reset"/>.
         */
        [JsonIgnore]
        internal static readonly Lazy<T> EmptySaveData = new Lazy<T>(() => Construct("EMPTY"));

        /**
         * Stores the json representation of <see cref="EmptySaveData"/> for use with <see cref="Reset"/>.
         *
         * TODO: It's possible that <see cref="JsonConvert"/> respects <see cref="System.ComponentModel.DefaultValueAttribute"/> via <see cref="DefaultValueHandling"/>, which would probably be waaaaaaaaaaaaaaaaay more efficient and reasonable than this
         */
        [JsonIgnore]
        internal static readonly Lazy<string> EmptyJson = new Lazy<string>(() => EmptySaveData.Value.ToJson());

        /// <summary>
        ///     Static initializer that makes sure the <see cref="SaveFolderPath" /> exists.
        /// </summary>
        static SaveData() {
            if (!Directory.Exists(SaveFolderPath)) {
                Log(Color.yellow, $"{nameof(SaveFolderPath)} at {SaveFolderPath} didn't exist, so it is being created...");
                Directory.CreateDirectory(SaveFolderPath);
            }
        }

        protected SaveData(string nickname) {
            Nickname = nickname;
        }

        public static T Load(string nickName) {
            Log($"Loading save file: {nickName}");
            if (!SaveFileExists(nickName)) {
                throw new SaveDataException($"Attempt to load {typeof(T)} failed: No save files with the nickname {nickName} exist!");
            }

            var latestSaveFilePath = GetAllSaveFilePaths(nickName).Last();
            Log($"Found latest save file for {nickName} at path: {latestSaveFilePath}");

            var deserializedSaveFile = DeserializeByPath(latestSaveFilePath);
            deserializedSaveFile.OnLoadPrivate();
            return deserializedSaveFile;
        }

        public static T LoadByPath(string path) {
            Log($"Loading save file at path: {path}");
            var deserializedSaveFile = DeserializeByPath(path);
            deserializedSaveFile.OnLoadPrivate();
            return deserializedSaveFile;
        }

        public T Reload() {
            Log($"Reloading save file: {Nickname}");
            Reset();
            JsonConvert.PopulateObject(GetSaveFileContent(LatestSaveFilePath), this, CurrentJsonSerializerSettings);
            OnLoadPrivate();
            return (T)this;
        }

        public T Reset() {
            var oldNickname = Nickname;
            JsonConvert.PopulateObject(EmptyJson.Value, this, CurrentJsonSerializerSettings);
            Nickname = oldNickname;
            return (T)this;
        }

        /// <summary>
        /// Attempts to <see cref="Load"/> the <see cref="SaveData{T}"/> with the <see cref="Nickname"/> <paramref name="nickName"/>, storing it in <paramref name="saveData"/>.
        ///
        /// Returns <c>true</c> or <c>false</c> based on whether or not the <see cref="Load"/> succeeded.
        /// </summary>
        /// <param name="nickName">The <see cref="Nickname"/> of the <see cref="SaveData{T}"/> you would like to <see cref="Load"/>.</param>
        /// <param name="saveData">Holds the <see cref="SaveData{T}"/>, if the <see cref="Load"/> was a success; otherwise, <c>null</c>.</param>
        /// <returns></returns>
        public static bool TryLoad(string nickName, out T saveData) {
            try {
                saveData = Load(nickName);
                return true;
            }
            catch (SaveDataException) {
                saveData = null;
                return false;
            }
        }

        private static T DeserializeByContent(string saveFileContent) {
            try {
                return JsonConvert.DeserializeObject<T>(saveFileContent, CurrentJsonSerializerSettings);
            }
            catch (JsonException e) {
                throw new SaveDataException(
                    $"Unable to {nameof(DeserializeByContent)} the provided {nameof(saveFileContent)}!\n\tContent:{saveFileContent}\n",
                    e
                );
            }
        }

        private static string GetSaveFileContent(string saveFilePath) {
            try {
                return File.ReadAllText(saveFilePath);
            }
            catch (FileNotFoundException e) {
                throw new SaveDataException($"No save file exists at the path {saveFilePath}", e);
            }
        }

        private static T DeserializeByPath(string saveFilePath) {
            try {
                return DeserializeByContent(GetSaveFileContent(saveFilePath));
            }
            catch (FileNotFoundException e) {
                throw new SaveDataException($"No save file exists to {nameof(DeserializeByPath)} at path {saveFilePath}", e);
            }
        }

        /// <summary>
        /// Non-overrideable method called whenever a <see cref="Load"/> (or related method, like <see cref="Reload"/>) is called.
        /// </summary>
        /// <remarks>
        /// This contains logic that <b>must never be overriden</b> - preventing errors in case an inheritor forgets to call <c>base.OnLoad()</c> in their overload of <see cref="OnLoad"/>.
        /// </remarks>
        /// <seealso cref="OnLoad"/>
        private void OnLoadPrivate() {
            LastLoadTime = FrameTime.Now;
            OnLoad();
        }

        /// <summary>
        /// Overrideable method called whenever a <see cref="Load"/> (or related method, like <see cref="Reload"/>) is called.
        /// </summary>
        protected virtual void OnLoad() {
            //no-op
        }

        /// <summary>
        ///     Gets the path to a <b>theoretical</b> save file with the given <c>nickName</c> and <see cref="DateTime" />-stamp.
        ///     <br />
        ///     This method does <b>not</b> know or care if the save file exists!
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [UsedImplicitly]
        public static string GetSaveFilePath(string nickName, DateTime dateTime) {
            return Path.ChangeExtension(Path.Combine(SaveFolderPath, GetSaveFileNameWithDate(nickName, dateTime)), SaveData.SaveFileExtension);
        }

        /// <summary>
        /// Returns <c>true</c> if <b>any</b> files exist with the given <see cref="Nickname"/>.
        /// </summary>
        /// <param name="nickname">the expected <see cref="Nickname"/></param>
        /// <returns></returns>
        [UsedImplicitly]
        public static bool SaveFileExists(string nickname) {
            return GetAllSaveFilePaths(nickname).Any(File.Exists);
        }

        /// <summary>
        ///     Creates a new, blank <see cref="SaveData{T}" /> of type <see cref="T" />, and <see cref="Save(BrandonUtils.Saving.SaveData{T},string,bool)" />s it as a new file with the <see cref="Nickname" /> <paramref name="nickname" />.
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns>the newly created <see cref="SaveData{T}" /></returns>
        public static T NewSaveFile(string nickname) {
            Log(
                $"Creating a new save file:",
                $"{nameof(Nickname)}: {nickname}",
                $"type: {typeof(T).Name}",
                $"folder: {SaveFolderPath}"
            );

            //create the save folder if it doesn't already exist
            Directory.CreateDirectory(
                SaveFolderPath ??
                throw new SaveDataException(
                    $"The path {SaveFolderPath} didn't have a valid directory name!",
                    new DirectoryNotFoundException()
                )
            );

            //create a new, blank save data, and save it as the new file
            return Save(Construct(nickname), nickname, false);
        }

        /// <summary>
        /// Constructs a new instance of <see cref="T"/> with the given <see cref="Nickname"/>.
        /// </summary>
        /// <remarks>
        /// Calling this from a child class, like <see cref="SaveDataTestImpl"/>, should create an instance of the child class.
        /// </remarks>
        /// <param name="nickname">the desired <see cref="Nickname"/></param>
        /// <returns>a new instance of <see cref="T"/></returns>
        public static T Construct(string nickname) {
            return ReflectionUtils.Construct<T>(nickname);
        }

        /// <summary>
        ///     Serializes <paramref name="saveData" /> to a new <see cref="SaveData.SaveFileExtension"/> <see cref="File"/>.
        /// </summary>
        /// <remarks>
        ///     <para>The new file will be located at <see cref="GetSaveFilePath"/>.</para>
        ///     <para>Retains previous saves with the same <paramref name="nickname"/>, up to <see cref="SaveData.BackupSaveSlots"/>, via <see cref="TrimSaves"/>.</para>
        ///     <para>May update fields in <paramref name="saveData"/>, such as <see cref="LastSaveTime" />.</para>
        /// </remarks>
        /// <param name="saveData">The <see cref="SaveData{T}"/> inheritor to be saved.</param>
        /// <param name="nickname">The <see cref="Nickname"/> that the <see cref="saveData"/> should be given.</param>
        /// <param name="useReSaveDelay">If <c>true</c>, check if <see cref="SaveData.ReSaveDelay"/> has elapsed since <see cref="LastSaveTime"/>.</param>
        /// <returns>The passed <paramref name="saveData" /> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="saveData"/> is null</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="nickname"/> <see cref="string.IsNullOrWhiteSpace"/></exception>
        /// <exception cref="SaveDataException">If a file at <see cref="GetSaveFilePath"/> already exists</exception>
        /// <exception cref="ReSaveDelayException">If <paramref name="useReSaveDelay"/> is <c>true</c> and <see cref="SaveData.ReSaveDelay"/> hasn't elapsed since <see cref="LastSaveTime"/></exception>
        private static T Save(SaveData<T> saveData, string nickname, bool useReSaveDelay = true) {
            if (saveData == null) {
                throw new ArgumentNullException(nameof(saveData));
            }

            if (string.IsNullOrWhiteSpace(nickname)) {
                throw new ArgumentException($"The name of the file you tried to save was '{nickname}', which is null, blank, or whitespace, so we can't save it!", nameof(nickname));
            }

            var saveTime = DateTime.Now;

            //throw an error if ReSaveDelay hasn't elapsed since the last time the file was saved
            if (useReSaveDelay && saveTime - saveData.LastSaveTime_Exact < SaveData.ReSaveDelay) {
                throw new ReSaveDelayException(
                    saveData,
                    $"The save file {nickname} was saved too recently!"                         +
                    $"\n\t{nameof(saveData.LastSaveTime_Exact)}: {saveData.LastSaveTime_Exact}" +
                    $"\n\tNew {nameof(saveTime)}: {saveTime}"                                   +
                    $"\n\t{nameof(SaveData.ReSaveDelay)}: {SaveData.ReSaveDelay}"               +
                    $"\n\tDelta: {saveTime - saveData.LastSaveTime_Exact}"
                );
            }

            saveData.Nickname           = nickname;
            saveData.LastSaveTime_Exact = saveTime;
            saveData.LastSaveTime       = FrameTime.Now;

            var previousFileCount = saveData.AllSaveFilePaths.Length;
            var newFilePath       = GetSaveFilePath(nickname, saveTime);

            //Make sure that the file we're trying to create doesn't already exist
            if (File.Exists(newFilePath)) {
                throw new SaveDataException(saveData, $"Couldn't save {nickname} because there was already a save file at the path {newFilePath}");
            }

            //Write to the new save file
            File.WriteAllText(newFilePath, saveData.ToJson());
            if (!File.Exists(newFilePath)) {
                throw new FileNotFoundException("Couldn't create the new save file!", newFilePath);
            }

            if (saveData.AllSaveFilePaths.Length <= previousFileCount) {
                throw new SaveDataException(
                    saveData,
                    $"When saving {nickname}, we failed to create a new file!" +
                    $"\n\t{nameof(previousFileCount)} = {previousFileCount}"   +
                    $"\n\tcurrentFileCount = {saveData.AllSaveFilePaths.Length}"
                );
            }

            Log($"Finished saving {nickname}! Trimming previous saves down to {SaveData.BackupSaveSlots}...");
            TrimSaves(nickname);

            return saveData as T;
        }

        /// <summary>
        ///     Calls the static <see cref="Save(BrandonUtils.Saving.SaveData{T},string,bool)" /> with this <see cref="SaveData{T}" />'s <see cref="Nickname" />.
        /// </summary>
        /// <inheritdoc cref="Save(BrandonUtils.Saving.SaveData{T},string,bool)"/>
        /// <returns></returns>
        public void Save(bool useReSaveDelay = true) {
            Save(this, Nickname, useReSaveDelay);
        }

        /// <summary>
        /// Overrideable method called whenever the file is <see cref="Save(BrandonUtils.Saving.SaveData{T},string,bool)"/>ed.
        /// </summary>
        protected virtual void OnSave() {
            //no-op
        }

        public static void TrimSaves(string nickName, int trimTo = BackupSaveSlots) {
            LogUtils.Log($"Trimming save files named '{nickName}' down to {trimTo} saves");
            var saveFiles = GetAllSaveFilePaths(nickName);
            if (saveFiles.Length <= trimTo) {
                LogUtils.Log($"There were {saveFiles.Length} files, which is less than the requested trim count of {trimTo}, so we aren't going to trim anything.");
                return;
            }

            //A for loop is used here instead of a while loop in order to guard against infinite loops (basically, while loops scare me)
            var toDelete = saveFiles.Length - trimTo;
            for (var i = 0; i < toDelete; i++) {
                Delete(GetAllSaveFilePaths(nickName).First());
                saveFiles = GetAllSaveFilePaths(nickName);
            }
        }

        /// <summary>
        ///     Returns the <b><see cref="string" /> paths</b> to all of the save files for the given <paramref name="nickName"/> that <b>currently exist</b>.
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public static string[] GetAllSaveFilePaths(string nickName) {
            //This used to use "" as a default value for nickName, and I don't know why...
            var saveFiles = Directory.GetFiles(SaveFolderPath, $"{nickName}*{SaveData.SaveFileExtension}");
            SortSaveFilePaths(saveFiles);
            return saveFiles;
        }

        public static string GetLatestSaveFilePath(string nickName) {
            var allPaths = GetAllSaveFilePaths(nickName);
            if (allPaths.Length == 0) {
                throw new SaveDataException($"Unable to retrieve the latest save file path because no files exist with the {nameof(Nickname)} {nickName}!");
            }

            return allPaths.Last();
        }

        public static string GetOldestSaveFilePath(string nickName) {
            var allPaths = GetAllSaveFilePaths(nickName);
            if (allPaths.Length == 0) {
                throw new SaveDataException($"Unable to retrieve the oldest save file path because no files exist with the {nameof(Nickname)} {nickName}!");
            }

            return allPaths.First();
        }

        /// <summary>
        ///     Sorts the given save file paths<b><i>CHRONOLOGICALLY</i></b>by their <see cref="GetSaveDate" />.
        /// </summary>
        /// <param name="saveFilePaths"></param>
        private static void SortSaveFilePaths(string[] saveFilePaths) {
            Array.Sort(saveFilePaths, (save1, save2) => GetSaveDate(save1).CompareTo(GetSaveDate(save2)));
        }

        public static string GetSaveFileNameWithDate(string nickName, DateTime saveDate) {
            return $"{nickName}_{GetTimeStamp(saveDate)}";
        }

        /// <summary>
        ///     Converts a <see cref="DateTime" /> to a standardized, file-name-friendly "Time Stamp".
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime" /> to be converted.</param>
        /// <returns>A file-name-friendly "Time Stamp" string.</returns>
        /// <seealso cref="SaveData.TimeStampLength" />
        /// <seealso cref="SaveData.TimeStampPattern" />
        public static string GetTimeStamp(DateTime dateTime) {
            return dateTime.Ticks.ToString().PadLeft(18, '0');
        }

        public static string GetNickname(string saveFileName) {
            return Regex.Match(saveFileName, SaveFilePattern).Groups["nickName"].Value;
        }

        public static DateTime GetSaveDate(string saveFileName) {
            var dateString = Regex.Match(saveFileName, SaveFilePattern).Groups["date"].Value;

            try {
                var ticks    = long.Parse(dateString);
                var saveDate = new DateTime(long.Parse(dateString));
                return saveDate;
            }
            catch (Exception e) {
                throw new SaveDataException($"Could not parse the time stamp from {nameof(saveFileName)} {saveFileName}!" + $"\n\t{nameof(dateString)} was extracted as: [{dateString}]", e);
            }
        }

        public static bool Delete(string saveFilePath) {
            if (File.Exists(saveFilePath)) {
                Log(Color.yellow, $"About to delete the save file: {saveFilePath}");
                File.Delete(saveFilePath);
                return true;
            }

            Log($"Can't delete the save file because it doesn't exist! {saveFilePath}");
            return false;
        }

        /// <summary>
        ///     The canon way to convert a <see cref="SaveData{T}"/> to a json.
        /// </summary>
        /// <returns></returns>
        public string ToJson() {
            return JsonConvert.SerializeObject(this, CurrentJsonSerializerSettings);
        }

        public override string ToString() {
            return ToJson();
        }
    }
}