using System;

using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings.Json;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BrandonUtils.Standalone.Strings {
    public class PrettificationSettings : IJsonCloneable {
        public static Func<PrettificationSettings> DefaultSupplier = () => new PrettificationSettings();

        /// <summary>
        /// When the <see cref="PreferredLineStyle"/> is <see cref="LineStyle.Dynamic"/>, <see cref="LineLengthLimit"/> is used to decide between <see cref="LineStyle.Multi"/> and <see cref="LineStyle.Single"/>.
        /// </summary>
        [NotNull]
        public Fallback<int> LineLengthLimit { get; } = new Fallback<int>(70);

        [NotNull] public Fallback<string> TableHeaderSeparator { get; } = new Fallback<string>("-");
        [NotNull] public Fallback<string> TableColumnSeparator { get; } = new Fallback<string>(" ");
        [NotNull] public Fallback<string> NullPlaceholder      { get; } = new Fallback<string>("⛔");

        /// <summary>
        /// The preferred <see cref="LineStyle"/>.
        /// </summary>
        [NotNull]
        public Fallback<LineStyle> PreferredLineStyle { get; } = new Fallback<LineStyle>(LineStyle.Dynamic);

        [NotNull] public Fallback<TypeNameStyle> TypeLabelStyle { get; } = new Fallback<TypeNameStyle>(TypeNameStyle.Full);

        [NotNull] public Fallback<TypeNameStyle> EnumLabelStyle { get; } = new Fallback<TypeNameStyle>(TypeNameStyle.None);

        [NotNull] public Fallback<HeaderStyle> HeaderStyle { get; } = new Fallback<HeaderStyle>(Strings.HeaderStyle.None);

        [NotNull]
        public static PrettificationSettings GetDefault([CanBeNull] Action<PrettificationSettings> modifications = default) {
            var settings = DefaultSupplier.Invoke();
            modifications?.Invoke(settings);
            return settings;
        }

        [NotNull]
        public static PrettificationSettings GetEmpty([CanBeNull] Action<PrettificationSettings> modifications = default) {
            var settings = new PrettificationSettings();
            modifications?.Invoke(settings);
            return settings;
        }

        [NotNull]
        public static implicit operator PrettificationSettings(LineStyle lineStyle) {
            return GetDefault(it => it.PreferredLineStyle.Set(lineStyle));
        }

        [NotNull]
        public static implicit operator PrettificationSettings(TypeNameStyle typeLabelStyle) {
            return GetDefault(it => it.TypeLabelStyle.Set(typeLabelStyle));
        }

        [NotNull]
        public static implicit operator PrettificationSettings(HeaderStyle headerStyle) {
            return GetDefault(it => it.HeaderStyle.Set(headerStyle));
        }

        [CanBeNull]
        [JsonIgnore]
        public ITraceWriter TraceWriter { get; set; } = null;

        [NotNull]
        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}