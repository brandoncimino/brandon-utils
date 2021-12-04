using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Strings.Json;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        [NotNull]
        private static IList<T> AsList<T>([NotNull] this IEnumerable<T> enumerable) {
            return enumerable as IList<T> ?? enumerable.ToList();
        }

        public static string PrettifyEnumerable(
            [NotNull] [ItemCanBeNull]
            IEnumerable enumerable,
            [NotNull] PrettificationSettings settings
        ) {
            var asObjects      = enumerable.Cast<object>();
            var enumerableType = enumerable.GetType();
            var lineStyle      = settings?.PreferredLineStyle.Value ?? default;
            var innerSettings = settings.JsonClone(
                it => {
                    it.EnumLabelStyle.Set(TypeNameStyle.None);
                    it.TypeLabelStyle.Set(it.TypeLabelStyle.Value.Reduce());
                }
            );
            return lineStyle switch {
                LineStyle.Dynamic => PrettifyEnumerable_DynamicLine(asObjects, enumerableType, settings, innerSettings),
                LineStyle.Multi   => PrettifyEnumerable_MultiLine(asObjects, enumerableType, settings, innerSettings),
                LineStyle.Single  => PrettifyEnumerable_SingleLine(asObjects, enumerableType, settings, innerSettings),
                _                 => throw BEnum.InvalidEnumArgumentException(nameof(PrettificationSettings.PreferredLineStyle), lineStyle)
            };
        }

        private static string PrettifyEnumerable_DynamicLine<T>(
            [NotNull] [ItemCanBeNull]
            IEnumerable<T> enumerable,
            [NotNull] Type                   enumerableType,
            [NotNull] PrettificationSettings outerSettings,
            [NotNull] PrettificationSettings innerSettings
        ) {
            outerSettings.TraceWriter.Verbose(() => $"🎨 via {nameof(PrettifyEnumerable_DynamicLine)}");
            enumerable = enumerable.AsList();

            var single = PrettifyEnumerable_SingleLine(enumerable, enumerableType, outerSettings, innerSettings);

            if (single.Length > outerSettings.LineLengthLimit) {
                outerSettings.TraceWriter.Verbose(() => $"🪓 {nameof(single)} length {single.Length} > {outerSettings.LineLengthLimit}/{outerSettings.LineLengthLimit.Value}/{(int)outerSettings.LineLengthLimit}; falling back to {nameof(PrettifyEnumerable_MultiLine)}", 1);
                return PrettifyEnumerable_MultiLine(enumerable, enumerableType, outerSettings, innerSettings);
            }

            return single;
        }

        private static string PrettifyEnumerable_MultiLine<T>(
            [NotNull] [ItemCanBeNull]
            IEnumerable<T> enumerable,
            [NotNull] Type                   enumerableType,
            [NotNull] PrettificationSettings outerSettings,
            [NotNull] PrettificationSettings innerSettings
        ) {
            outerSettings.TraceWriter.Verbose(() => $"🎨 via {nameof(PrettifyEnumerable_MultiLine)}");
            enumerable = enumerable.AsList();

            return enumerable.Select(it => it.Prettify(innerSettings).Indent())
                             .Bookend("[", "]")
                             .JoinLines()
                             .WithTypeLabel(enumerableType, outerSettings);
        }

        private static string PrettifyEnumerable_SingleLine<T>(
            [NotNull] [ItemCanBeNull]
            IEnumerable<T> enumerable,
            [NotNull] Type                   enumerableType,
            [NotNull] PrettificationSettings outerSettings,
            [NotNull] PrettificationSettings innerSettings
        ) {
            outerSettings.TraceWriter.Verbose(() => $"🎨 via {nameof(PrettifyEnumerable_SingleLine)}");
            var joined = enumerable.Select(it => it.Prettify(innerSettings)).JoinString(", ");
            return $"[{joined}]".WithTypeLabel(enumerableType, outerSettings);
        }
    }
}