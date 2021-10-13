using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;

using FowlFever.Conjugal.Affixing;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        [NotNull]
        private static string PrettifyTupleArray([NotNull] this IEnumerable<object> array, [CanBeNull] PrettificationSettings settings) {
            settings ??= Prettification.DefaultPrettificationSettings;
            return array
                   .Select(it => it.Prettify(settings))
                   .JoinString(", ")
                   .Circumfix("(", ")");
        }

        [NotNull, Pure]
        public static string Tuple2<T1, T2>((T1, T2) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple3<T, T2, T3>((T, T2, T3) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple4<T, T2, T3, T4>((T, T2, T3, T4) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple5<T, T2, T3, T4, T5>((T, T2, T3, T4, T5) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple6<T, T2, T3, T4, T5, T6>((T, T2, T3, T4, T5, T6) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple7<T, T2, T3, T4, T5, T6, T7>((T, T2, T3, T4, T5, T6, T7) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple8<T, T2, T3, T4, T5, T6, T7, T8>((T, T2, T3, T4, T5, T6, T7, T8) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple9<T, T2, T3, T4, T5, T6, T7, T8, T9>((T, T2, T3, T4, T5, T6, T7, T8, T9) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple10<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple11<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple12<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple13<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple14<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple15<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple16<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);

        [NotNull, Pure]
        public static string Tuple17<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>((T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17) tuple, [CanBeNull] PrettificationSettings settings = default) => tuple.ToArray().PrettifyTupleArray(settings);
    }
}