using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        public static string PrettifyEnumerableT<T>([NotNull] [ItemCanBeNull] IEnumerable<T> enumerable, [CanBeNull] PrettificationSettings settings) {
            var lineStyle = settings?.PreferredLineStyle ?? default;
            return lineStyle switch {
                PrettificationSettings.LineStyle.Multi   => PrettifyEnumerable_MultiLine(enumerable, enumerable.GetType(), settings),
                PrettificationSettings.LineStyle.Single  => PrettifyEnumerable_SingleLine(enumerable, enumerable.GetType(), settings),
                PrettificationSettings.LineStyle.Dynamic => PrettifyEnumerable_DynamicLine(enumerable, enumerable.GetType(), settings),
                _                                        => throw BEnum.InvalidEnumArgumentException(nameof(PrettificationSettings.PreferredLineStyle), lineStyle)
            };
        }

        private static string PrettifyEnumerable_DynamicLine<T>(
            [NotNull] [ItemCanBeNull]
            IEnumerable<T> enumerable,
            [NotNull] Type enumerableType,
            [CanBeNull]
            PrettificationSettings settings
        ) {
            enumerable = enumerable.ToList();
            var single = PrettifyEnumerable_SingleLine(enumerable, enumerableType, settings);
            return single.Length > settings?.LineLengthLimit ? PrettifyEnumerable_MultiLine(enumerable, enumerableType, settings) : single;
        }

        private static string PrettifyEnumerable_MultiLine<T>(
            [NotNull] [ItemCanBeNull]
            IEnumerable<T> enumerable,
            [NotNull] Type enumerableType,
            [CanBeNull]
            PrettificationSettings prettificationSettings
        ) {
            enumerable = enumerable.ToList();
            var str = enumerable.Select(it => it.Prettify().Indent())
                                .Prepend("[")
                                .Append("]")
                                .JoinLines();

            if (prettificationSettings?.Flags.HasFlag(PrettificationFlags.IncludeTypeLabels) == true) {
                var typeLabel = enumerableType.PrettifyType();
                str = $"{typeLabel}{str}";
            }

            return str;
        }

        public static string PrettifyEnumerable([NotNull] [ItemCanBeNull] IEnumerable enumerable, [CanBeNull] PrettificationSettings settings) {
            var asObjects      = enumerable.Cast<object>();
            var enumerableType = enumerable.GetType();
            var lineStyle      = settings?.PreferredLineStyle ?? default;
            return lineStyle switch {
                PrettificationSettings.LineStyle.Dynamic => PrettifyEnumerable_DynamicLine(asObjects, enumerableType, settings),
                PrettificationSettings.LineStyle.Multi   => PrettifyEnumerable_MultiLine(asObjects, enumerableType, settings),
                PrettificationSettings.LineStyle.Single  => PrettifyEnumerable_SingleLine(asObjects, enumerableType, settings),
                _                                        => throw BEnum.InvalidEnumArgumentException(nameof(PrettificationSettings.PreferredLineStyle), lineStyle)
            };
        }

        private static string PrettifyEnumerable_SingleLine<T>(
            [NotNull] [ItemCanBeNull]
            IEnumerable<T> enumerable,
            [NotNull] Type enumerableType,
            [CanBeNull]
            PrettificationSettings prettificationSettings
        ) {
            enumerable = enumerable.ToList();
            var joined = enumerable.Select(it => it.Prettify()).JoinString(", ");
            var str    = $"[{joined}]";

            if (prettificationSettings?.Flags.HasFlag(PrettificationFlags.IncludeTypeLabels) == true) {
                var typeLabel = enumerableType.PrettifyType();
                str = $"{typeLabel}{str}";
            }

            return str;
        }
    }
}