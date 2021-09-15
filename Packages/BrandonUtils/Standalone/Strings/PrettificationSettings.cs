using BrandonUtils.Standalone.Optional;

namespace BrandonUtils.Standalone.Strings {
    public class PrettificationSettings {
        public enum LineStyle {
            Dynamic = default,
            Multi,
            Single,
        }

        /// <summary>
        /// When the <see cref="PreferredLineStyle"/> is <see cref="LineStyle.Dynamic"/>, <see cref="LineLengthLimit"/> is used to decide between <see cref="LineStyle.Multi"/> and <see cref="LineStyle.Single"/>.
        /// </summary>
        public int? LineLengthLimit { get; set; }

        /// <summary>
        /// Assorted mutually exclusive <see cref="PrettificationFlags"/>.
        /// </summary>
        public PrettificationFlags Flags { get;        set; }
        public Optional<string> NullPlaceholder { get; set; }

        /// <summary>
        /// The preferred <see cref="LineStyle"/>.
        /// </summary>
        public LineStyle? PreferredLineStyle { get; set; }

        public static implicit operator PrettificationSettings(LineStyle lineStyle) {
            return new PrettificationSettings() { PreferredLineStyle = lineStyle };
        }

        public static implicit operator PrettificationSettings(PrettificationFlags flags) {
            return new PrettificationSettings() { Flags = flags };
        }
    }
}