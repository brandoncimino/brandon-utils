using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Collections {
    [PublicAPI]
    public static class TupleExtensions {
        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T) tuple) => new[] { tuple.Item1, tuple.Item2 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16 };

        [Pure, NotNull, ItemCanBeNull]
        public static T[] ToArray<T>(this (T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T) tuple) => new[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16, tuple.Item17 };


        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2>(this (T, T2) tuple) => new object[] { tuple.Item1, tuple.Item2 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3>(this (T, T2, T3) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4>(this (T, T2, T3, T4) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5>(this (T, T2, T3, T4, T5) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6>(this (T, T2, T3, T4, T5, T6) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7>(this (T, T2, T3, T4, T5, T6, T7) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8>(this (T, T2, T3, T4, T5, T6, T7, T8) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9>(this (T, T2, T3, T4, T5, T6, T7, T8, T9) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16 };

        [Pure, NotNull, ItemCanBeNull]
        public static object[] ToArray<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(this (T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17) tuple) => new object[] { tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10, tuple.Item11, tuple.Item12, tuple.Item13, tuple.Item14, tuple.Item15, tuple.Item16, tuple.Item17 };
    }
}