using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical.Saving {
    /// <summary>
    /// Joins together a <see cref="System.IO.FileInfo"/> with the <see cref="ISaveData"/> it contains.
    /// </summary>
    /// <typeparam name="TData">the type of the serialized <see cref="Data"/></typeparam>
    [PublicAPI]
    public interface ISaveFile<out TData> : IHasFileInfo where TData : ISaveData {
        [NotNull] public string Nickname { get; }

        [NotNull] public ISaveFolder SaveFolder { get; }

        [CanBeNull] public TData Data { get; }

        /// <summary>
        /// The <see cref="DateTime"/> at which the <b><see cref="Data"/> refers to</b>.
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Serializes <see cref="Data"/> to <see cref="ISaveFile{TData}.File"/>.
        /// </summary>
        /// <param name="duplicateFileResolution">determines what we should do when a file already exists</param>
        /// <param name="jsonSettings">optional <see cref="JsonSerializerSettings"/></param>
        /// <returns></returns>
        public ISaveFile<TData> Save(DuplicateFileResolution duplicateFileResolution, [CanBeNull] JsonSerializerSettings jsonSettings = default);

        /**
         * <inheritdoc cref="Save(BrandonUtils.Standalone.Clerical.DuplicateFileResolution,Newtonsoft.Json.JsonSerializerSettings)"/>
         */
        public ISaveFile<TData> Save([CanBeNull] SaveManagerSettings saveSettings = default);

        [NotNull]
        public ISaveFile<TData> Load([CanBeNull] SaveManagerSettings saveSettings = default);
    }
}