namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        public static string Tuple2<T1, T2>((T1, T2) tuple2, PrettificationSettings settings = default) {
            var (a, b) = tuple2;
            return $"({a.Prettify()}, {b.Prettify()})";
        }
    }
}