using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        [NotNull]
        public static string Tuple2<T1, T2>((T1, T2) tuple2, [CanBeNull] PrettificationSettings settings = default) {
            var (a, b) = tuple2;
            return $"({a.Prettify(settings)}, {b.Prettify(settings)})";
        }
    }
}