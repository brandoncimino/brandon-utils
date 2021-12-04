using BrandonUtils.Standalone.Optional;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace BrandonUtils.Standalone.Clerical.Saving {
    [PublicAPI]
    public class SaveManagerSettings {
        [NotNull] public Fallback<JsonSerializerSettings?> JsonSerializerSettings  { get; } = new Fallback<JsonSerializerSettings>(new JsonSerializerSettings());
        [NotNull] public Fallback<string?>                 AutoSaveName            { get; } = new Fallback<string>("AutoSave");
        [NotNull] public Fallback<string?>                 SaveFileExtension       { get; } = new Fallback<string>(".sav.json");
        [NotNull] public Fallback<int>                     BackupSaveSlots         { get; } = new Fallback<int>(10);
        [NotNull] public Fallback<DuplicateFileResolution> DuplicateFileResolution { get; } = new Fallback<DuplicateFileResolution>();
    }
}