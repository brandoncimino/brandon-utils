using BrandonUtils.Standalone.Optional;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    public class PrettificationSettings {
        /// <summary>
        /// When the <see cref="PreferredLineStyle"/> is <see cref="LineStyle.Dynamic"/>, <see cref="LineLengthLimit"/> is used to decide between <see cref="LineStyle.Multi"/> and <see cref="LineStyle.Single"/>.
        /// </summary>
        [NotNull]
        public Fallback<int> LineLengthLimit { get; } = new Fallback<int>(50);

        [NotNull] public Fallback<string> NullPlaceholder { get; } = new Fallback<string>("⛔");

        /// <summary>
        /// The preferred <see cref="LineStyle"/>.
        /// </summary>
        [NotNull]
        public Fallback<LineStyle> PreferredLineStyle { get; } = new Fallback<LineStyle>(LineStyle.Dynamic);

        [NotNull] public Fallback<TypeNameStyle> TypeLabelStyle { get; } = new Fallback<TypeNameStyle>(TypeNameStyle.Full);

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
    }
}