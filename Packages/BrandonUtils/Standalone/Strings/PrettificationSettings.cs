using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings.Json;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BrandonUtils.Standalone.Strings {
    public class PrettificationSettings : IJsonCloneable {
        /// <summary>
        /// When the <see cref="PreferredLineStyle"/> is <see cref="LineStyle.Dynamic"/>, <see cref="LineLengthLimit"/> is used to decide between <see cref="LineStyle.Multi"/> and <see cref="LineStyle.Single"/>.
        /// </summary>
        [NotNull]
        public Fallback<int> LineLengthLimit { get; } = new Fallback<int>(50);

        [NotNull] public Fallback<string> TableHeaderSeparator { get; } = new Fallback<string>("-");
        [NotNull] public Fallback<string> TableColumnSeparator { get; } = new Fallback<string>(" ");

        [NotNull] public Fallback<string> NullPlaceholder { get; } = new Fallback<string>("⛔");

        /// <summary>
        /// The preferred <see cref="LineStyle"/>.
        /// </summary>
        [NotNull]
        public Fallback<LineStyle> PreferredLineStyle { get; } = new Fallback<LineStyle>(LineStyle.Dynamic);

        [NotNull] public Fallback<TypeNameStyle> TypeLabelStyle { get; } = new Fallback<TypeNameStyle>(TypeNameStyle.Full);

        [NotNull] public Fallback<HeaderStyle> HeaderStyle { get; } = new Fallback<HeaderStyle>(Strings.HeaderStyle.None);

        [NotNull]
        public static implicit operator PrettificationSettings(LineStyle lineStyle) {
            return new PrettificationSettings() {
                PreferredLineStyle = { Value = lineStyle }
            };
        }

        [NotNull]
        public static implicit operator PrettificationSettings(TypeNameStyle typeLabelStyle) {
            return new PrettificationSettings() {
                TypeLabelStyle = { Value = typeLabelStyle }
            };
        }

        [NotNull]
        public static implicit operator PrettificationSettings(HeaderStyle headerStyle) {
            return new PrettificationSettings() {
                HeaderStyle = { Value = headerStyle }
            };
        }

        [CanBeNull] public ITraceWriter TraceWriter { get; set; } = null;

        [NotNull]
        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}