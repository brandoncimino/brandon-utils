using System;
using System.Diagnostics.Contracts;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Reflection;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Standalone.Strings {
    [PublicAPI]
    public static class Prettification {
        internal const          string                       DefaultNullPlaceholder = "⛔";
        private static readonly KeyedList<Type, IPrettifier> Prettifiers            = PrettifierDatabase.GetDefaultPrettifiers();

        [NotNull] private static PrettificationSettings _defaultSettings = new PrettificationSettings();
        [NotNull]
        public static PrettificationSettings DefaultPrettificationSettings {
            [NotNull] get => _defaultSettings;
            [CanBeNull] set => _defaultSettings = value ?? new PrettificationSettings();
        }

        public static void RegisterPrettifier([NotNull] IPrettifier prettifier) {
            if (prettifier == null) {
                throw new ArgumentNullException(nameof(prettifier));
            }

            Prettifiers.Add(prettifier);
        }

        [NotNull]
        public static IPrettifier UnregisterPrettifier([NotNull] Type prettifierType) {
            if (prettifierType == null) {
                throw new ArgumentNullException(nameof(prettifierType));
            }

            return Prettifiers.Grab(prettifierType);
        }

        #region Finding Prettifiers

        [Pure]
        [ItemNotNull]
        internal static Optional<IPrettifier> FindPrettifier([NotNull] Type type, [NotNull] PrettificationSettings settings) {
            if (settings.VerboseLogging) {
                Console.WriteLine($"🔎 Attempting to find an {nameof(IPrettifier)} for the type {type}");
            }

            var prettifier = Optional.Optional.FirstWithValue(
                (type, settings),
                FindExactPrettifier,
                FindExactGenericPrettifier,
                FindGenericallyTypedPrettifier,
                FindInheritedPrettifier
            );

            if (settings.VerboseLogging) {
                prettifier.IfPresentOrElse(
                    it => Console.WriteLine($"\t\t⛳ Found: {it}"),
                    () => Console.WriteLine("\t\t🐸 No prettifier found!")
                );
            }

            return prettifier;
        }

        #region Generics

        [ItemNotNull]
        internal static Optional<IPrettifier> FindGenericallyTypedPrettifier([NotNull] Type type, [NotNull] PrettificationSettings settings) {
            if (settings.VerboseLogging) {
                Console.WriteLine($"\t→ {nameof(FindGenericallyTypedPrettifier)}({type.Name})");
            }

            return Optional.Optional.FirstWithValue(
                (type, settings),
                FindExactGenericPrettifier,
                FindMatchingGenericPrettifier
            );
        }

        private static Optional<IPrettifier> FindExactGenericPrettifier([NotNull] Type type, [NotNull] PrettificationSettings settings) {
            return type.IsGenericTypeOrDefinition() == false ? default : FindExactPrettifier(type.GetGenericTypeDefinition(), settings);
        }

        internal static Optional<IPrettifier> FindMatchingGenericPrettifier([NotNull] Type type, [NotNull] PrettificationSettings settings) {
            return type.IsGenericTypeOrDefinition() ? Prettifiers.FindFirst(it => GenericTypesMatch(it.PrettifierType, type)) : default;
        }

        internal static bool GenericTypesMatch([NotNull] Type t1, [NotNull] Type t2) {
            if (!t1.IsGenericTypeOrDefinition() || !t2.IsGenericTypeOrDefinition()) {
                return false;
            }

            if (t1.GenericTypeArguments.Length != t2.GenericTypeArguments.Length) {
                return false;
            }

            try {
                var t1_ofObjs = MakeGenericOfObjects(t1);
                var t2_ofObjs = MakeGenericOfObjects(t2);

                return t1_ofObjs == t2_ofObjs || t1_ofObjs.IsAssignableFrom(t2_ofObjs) || t2_ofObjs.IsAssignableFrom(t1_ofObjs);
            }
            catch (Exception e) {
                throw e.ModifyMessage($"Unable to compare the generic types {t1} and {t2}!\n{e.Message}");
            }
        }

        [Pure]
        [NotNull]
        private static Type MakeGenericOfObjects([NotNull] Type type) {
            if (!type.IsGenericTypeOrDefinition()) {
                throw new ArgumentException("Must be a generic type or definition!", nameof(type));
            }

            var genArgCount = type.GetGenericArguments().Length;
            return type.GetGenericTypeDefinition().MakeGenericType(genArgCount.Repeat(typeof(object)).ToArray());
        }

        #endregion

        [Pure]
        [ItemNotNull]
        private static Optional<IPrettifier> FindExactPrettifier([NotNull] Type type, [NotNull] PrettificationSettings settings) {
            if (settings.VerboseLogging) {
                Console.WriteLine($"\t→ {nameof(FindExactPrettifier)}({type.Name})");
            }

            return Prettifiers.Find(type);
        }

        [Pure]
        [ItemNotNull]
        private static Optional<IPrettifier> FindInheritedPrettifier([NotNull] Type type, [NotNull] PrettificationSettings settings) {
            if (settings.VerboseLogging) {
                Console.WriteLine($"\t→ {nameof(FindInheritedPrettifier)}({type.Name})");
            }

            try {
                return new Optional<IPrettifier>(Prettifiers.First(it => it.PrettifierType.IsAssignableFrom(type)));
            }
            catch (InvalidOperationException) {
                return default;
            }
        }

        [Pure]
        [ItemNotNull]
        private static Optional<IPrettifier> FindEnumPrettifier([NotNull] Type type, [NotNull] PrettificationSettings settings) {
            if (settings.VerboseLogging) {
                Console.WriteLine($"\t→ {nameof(FindEnumPrettifier)}({type.Name})");
            }

            if (type.IsEnum) {
                return new Optional<IPrettifier>(PrettifierDatabase.EnumPrettifier);
            }

            throw new NotImplementedException("TODO over the weekend at nicole's place maybe");
        }

        #endregion

        [Pure]
        [NotNull]
        public static string Prettify([CanBeNull] this object cinderella, [CanBeNull] PrettificationSettings settings = default) {
            settings ??= DefaultPrettificationSettings;

            if (settings.VerboseLogging) {
                Console.WriteLine($"👸 Prettifying [{cinderella?.GetType()}]{cinderella}");
            }

            if (cinderella == null) {
                return settings.NullPlaceholder.Value ?? "";
            }

            var prettifier = FindPrettifier(cinderella.GetType(), settings);

            return prettifier
                .IfPresentOrElse(
                    it => it.PrettifySafely(cinderella, settings),
                    () => LastResortPrettifier(cinderella, settings)
                );
        }

        [NotNull]
        internal static string LastResortPrettifier([CanBeNull] object cinderella, [CanBeNull] PrettificationSettings settings) {
            settings ??= new PrettificationSettings();

            if (settings.VerboseLogging) {
                Console.WriteLine($"⛑ Using the LAST RESORT prettifier for [{cinderella?.GetType()}]{cinderella}: {nameof(Convert.ToString)}!");
            }

            return Convert.ToString(cinderella);
        }
    }
}