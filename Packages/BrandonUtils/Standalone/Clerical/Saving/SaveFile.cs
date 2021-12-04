using System;
using System.IO;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveFile<TData> : BaseSaveFile<TData> where TData : ISaveData {
        [NotNull] private readonly SaveFileName _saveFileName;

        public override string Nickname => _saveFileName.Nickname;

        public override DateTime TimeStamp => _saveFileName.TimeStamp;

        public SaveFile(
            [NotNull] ISaveFolder folder,
            [NotNull] string      nickname,
            DateTime              timeStamp,
            string?            extension = SaveFileName.DefaultExtension,
            TData                 data      = default
        )
            : this(
                folder,
                new SaveFileName {
                    Nickname      = nickname,
                    TimeStamp     = timeStamp,
                    FullExtension = extension ?? SaveFileName.DefaultExtension
                },
                data
            ) { }

        internal SaveFile(
            [NotNull]   ISaveFolder  folder,
            [NotNull]   SaveFileName saveFileName,
            [CanBeNull] TData        data = default
        ) : base(
            folder,
            saveFileName.Rendered.MustNotBeBlank(),
            data
        ) {
            _saveFileName = saveFileName ?? throw new ArgumentNullException(nameof(saveFileName));
        }

        public SaveFile(
            [NotNull]   ISaveFolder folder,
            [NotNull]   FileInfo    fileInfo,
            [CanBeNull] TData       data = default
        ) : base(
            folder,
            fileInfo,
            data
        ) {
            _saveFileName = SaveFileName.Parse(fileInfo);
        }
    }
}