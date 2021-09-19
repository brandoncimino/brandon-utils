using System;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical.Saving {
    public static class SaveFileExtensions {
        /// <summary>
        /// Deserializes the contents of a <see cref="SaveFile{TData}"/>'s <see cref="SaveFile{TData}.File"/> and stores it in its <see cref="SaveFile{TData}.Data"/>.
        /// </summary>
        /// <param name="saveFile">the <see cref="SaveFile{TData}"/> whose <see cref="SaveFile{TData}.File"/> is being deserialized</param>
        /// <param name="settings">optional <see cref="JsonSerializerSettings"/></param>
        /// <typeparam name="TData">the type of the <see cref="SaveFile{TData}"/>'s serialized <see cref="SaveFile{TData}.Data"/></typeparam>
        /// <returns></returns>
        public static void Load<TData>(this ISaveFile<TData> saveFile, JsonSerializerSettings settings = default) where TData : ISaveData {
            saveFile.SetDataInternal(saveFile.File.Deserialize<TData>(settings));
        }

        /// <summary>
        /// Serializes a <see cref="SaveFile{TData}"/>'s <see cref="SaveFile{TData}.Data"/> to its <see cref="SaveFile{TData}.File"/>.
        /// </summary>
        /// <param name="saveFile">the <see cref="SaveFile{TData}"/> whose <see cref="SaveFile{TData}.Data"/> is being serialized</param>
        /// <param name="now">the <see cref="DateTime"/> at which we are saving</param>
        /// <param name="settings">optional <see cref="JsonSerializerSettings"/></param>
        /// <typeparam name="TData">the type of the <see cref="SaveFile{TData}"/>'s serialized <see cref="SaveFile{TData}.Data"/></typeparam>
        /// <returns></returns>
        public static void Save<TData>(this ISaveFile<TData> saveFile, DateTime now, JsonSerializerSettings settings = default) where TData : ISaveData {
            saveFile.File.Serialize(saveFile.Data, settings);
        }
    }
}