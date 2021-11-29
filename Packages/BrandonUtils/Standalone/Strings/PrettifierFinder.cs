using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Reflection;
using BrandonUtils.Standalone.Strings.Json;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    [CanBeNull]
    public delegate Optional<IPrettifier> OptionalPrettifierFinder([NotNull] IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings);

    public delegate IPrettifier PrettifierFinder([NotNull] IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings);

    public static class PrettifierFinders {
        // [NotNull] public static readonly PrettifierFinder ExactPrettifier      = (db, t, s) => db.Find(t);
        // [NotNull] public static readonly PrettifierFinder FromPrettifiable     = (db, t, s) => t.Implements(typeof(IPrettifiable)) ? PrettifierDatabase.PrettifiablePrettifier : null;
        // [NotNull] public static readonly PrettifierFinder FromToString         = (db, t, s) => t.GetToStringOverride() != null ? PrettifierDatabase.ToStringPrettifier : null;
        // [NotNull] public static readonly PrettifierFinder ConsentingPrettifier = (db, t, s) => db.Find(it => it.CanPrettify(t));

        [NotNull]
        internal static OptionalPrettifierFinder[] GetDefaultFinders() => new OptionalPrettifierFinder[] {
            FindExactPrettifier,
            FindToStringOverridePrettifier,
            FindGenericallyTypedPrettifier,
            FindInheritedPrettifier
        };

        [System.Diagnostics.Contracts.Pure]
        [ItemNotNull]
        internal static Optional<IPrettifier> FindPrettifier(
            IPrettifierDatabase                prettifierDatabase,
            [NotNull]   Type                   type,
            [CanBeNull] PrettificationSettings settings,
            [NotNull, ItemNotNull, InstantHandle]
            IEnumerable<OptionalPrettifierFinder> finders
        ) {
            settings ??= Prettification.DefaultPrettificationSettings;
            settings.TraceWriter.Verbose(() => $"🔎 Attempting to find an {nameof(IPrettifier)} for the type {type}");
            var simplified = PrettificationTypeSimplifier.SimplifyType(type, settings, 0);

            var prettifier = Optional.Optional.FirstWithValue<OptionalPrettifierFinder, IPrettifier>(
                finders,
                prettifierDatabase,
                simplified,
                settings
            );

            settings.TraceWriter.Verbose(
                () => prettifier.IfPresentOrElse(
                    it => $"⛳ Found: {it}",
                    () => "🐸 No prettifier found!"
                ),
                2
            );

            return prettifier;
        }

        [ItemNotNull]
        internal static Optional<IPrettifier> FindGenericallyTypedPrettifier(IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindGenericallyTypedPrettifier)}({type.Name})", 1);

            return Optional.Optional.FirstWithValue<OptionalPrettifierFinder, IPrettifier>(
                new OptionalPrettifierFinder[] {
                    FindExactGenericPrettifier,
                    FindMatchingGenericPrettifier
                },
                prettifierDatabase,
                type,
                settings
            );
        }

        private static Optional<IPrettifier> FindExactGenericPrettifier(IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings) {
            return type.IsGenericType == false ? default : FindExactPrettifier(prettifierDatabase, type.GetGenericTypeDefinition(), settings);
        }

        internal static Optional<IPrettifier> FindMatchingGenericPrettifier(IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings) {
            IPrettifier found = type.IsGenericType ? prettifierDatabase.Find(it => GenericTypesMatch(it.PrettifierType, type)) : default;
            return Optional.Optional.OfNullable(found);
        }

        internal static bool GenericTypesMatch([NotNull] Type t1, [NotNull] Type t2) {
            if (t1.IsGenericType == false || t2.IsGenericType == false) {
                return false;
            }

            return t1.GetGenericTypeDefinition().IsAssignableFrom(t2.GetGenericTypeDefinition());
        }

        [System.Diagnostics.Contracts.Pure]
        [NotNull]
        private static Type MakeGenericOfObjects([NotNull] Type type) {
            if (!type.IsGenericType) {
                throw new ArgumentException("Must be a generic type or definition!", nameof(type));
            }

            var genArgCount = type.GetGenericArguments().Length;
            return type.GetGenericTypeDefinition().MakeGenericType(genArgCount.Repeat(typeof(object)).ToArray());
        }

        [System.Diagnostics.Contracts.Pure]
        [ItemNotNull]
        internal static Optional<IPrettifier> FindExactPrettifier([NotNull] IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindExactPrettifier)}({type.Name})", 1);
            return Optional.Optional.OfNullable(prettifierDatabase.Find(type));
        }

        [System.Diagnostics.Contracts.Pure]
        [ItemNotNull]
        internal static Optional<IPrettifier> FindToStringOverridePrettifier([CanBeNull] IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindToStringOverridePrettifier)}({type.Name})", 1);
            var toString = type.GetToStringOverride();
            return toString != null ? Optional.Optional.Of(PrettifierDatabase.ToStringPrettifier) : default;
        }

        internal static Optional<IPrettifier> FindInterfacePrettifier([NotNull] IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindInterfacePrettifier)}", 1);
            return Optional.Optional.OfNullable(prettifierDatabase.Find(it => it.PrettifierType.IsInterface && type.Implements(it.PrettifierType)));
        }

        [System.Diagnostics.Contracts.Pure]
        [ItemNotNull]
        internal static Optional<IPrettifier> FindInheritedPrettifier([NotNull] IPrettifierDatabase prettifierDatabase, [NotNull] Type type, [NotNull] PrettificationSettings settings) {
            settings.TraceWriter.Verbose(() => $"-> {nameof(FindInheritedPrettifier)}({type.Name})", 1);
            return Optional.Optional.OfNullable(prettifierDatabase.Find(it => it.PrettifierType.IsAssignableFrom(type)));
        }
    }
}