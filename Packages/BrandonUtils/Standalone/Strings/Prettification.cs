using System;
using System.Diagnostics.Contracts;

using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings.Json;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Strings {
    [PublicAPI]
    public static class Prettification {
        internal const           string              DefaultNullPlaceholder = "⛔";
        internal static readonly IPrettifierDatabase Prettifiers            = PrettifierDatabase.GetDefaultPrettifiers();

        [NotNull]
        internal static readonly OptionalPrettifierFinder[] Finders = {
            PrettifierFinders.FindExactPrettifier,
            PrettifierFinders.FindToStringOverridePrettifier,
            PrettifierFinders.FindGenericallyTypedPrettifier,
            PrettifierFinders.FindInheritedPrettifier
        };

        [NotNull] private static PrettificationSettings _defaultSettings = new PrettificationSettings();
        [NotNull]
        public static PrettificationSettings DefaultPrettificationSettings {
            [NotNull] get => _defaultSettings;
            [CanBeNull] set => _defaultSettings = value ?? new PrettificationSettings();
        }


        [NotNull]
        public static PrettificationSettings ResolveSettings([CanBeNull] PrettificationSettings settings) {
            return settings ?? DefaultPrettificationSettings;
        }

        public static void RegisterPrettifier([NotNull] IPrettifier prettifier) {
            Prettifiers.Register(prettifier);
        }

        [CanBeNull]
        public static IPrettifier UnregisterPrettifier([NotNull] Type prettifierType) {
            return Prettifiers.Deregister(prettifierType);
        }

        #region Finding Prettifiers

        #region Generics

        #endregion

        #endregion

        [Pure]
        [NotNull]
        public static string Prettify([CanBeNull] this object cinderella) {
            return cinderella.Prettify(default);
        }

        [Pure]
        [NotNull]
        public static string Prettify([CanBeNull] this object cinderella, [CanBeNull] PrettificationSettings settings) {
            settings = ResolveSettings(settings);

            settings.TraceWriter.Verbose(() => $"👸 Prettifying [{cinderella?.GetType()}]{cinderella}");

            if (cinderella == null) {
                return settings.NullPlaceholder.Value ?? "";
            }

            var prettifier = PrettifierFinders.FindPrettifier(
                Prettifiers,
                cinderella.GetType(),
                settings,
                Finders
            );

            return prettifier
                .IfPresentOrElse(
                    it => it.PrettifySafely(cinderella, settings),
                    () => LastResortPrettifier(cinderella, settings)
                );
        }

        [NotNull]
        internal static string LastResortPrettifier([CanBeNull] object cinderella, [CanBeNull] PrettificationSettings settings) {
            settings ??= new PrettificationSettings();

            settings.TraceWriter.Verbose(() => $"⛑ Using the LAST RESORT prettifier for [{cinderella?.GetType()}]{cinderella}: {nameof(Convert.ToString)}!");

            return Convert.ToString(cinderella);
        }
    }
}