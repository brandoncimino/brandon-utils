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

        public static void RegisterPrettifier(IPrettifier prettifier) {
            Prettifiers.Add(prettifier);
        }

        public static IPrettifier UnregisterPrettifier(Type prettifierType) {
            return Prettifiers.Grab(prettifierType);
        }

        internal static Optional<IPrettifier> FindGenericallyTypedPrettifier(Type type) {
            return type.IsGenericTypeOrDefinition() ? Prettifiers.FindFirst(it => GenericTypesMatch(it.PrettifierType, type)) : default;
        }

        private static Optional<IPrettifier> FindTuplePrettifier(Type type) {
            return Prettifiers.Where(it => it.PrettifierType.IsTupleType())
                              .Where(it => it.PrettifierType.GetGenericArguments().Length == type.GetGenericArguments().Length)
                              .FindFirst();
        }

        internal static bool GenericTypesMatch(Type t1, Type t2) {
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

        [Pure]
        [ItemNotNull]
        internal static Optional<IPrettifier> FindExactPrettifier(Type type) {
            return Prettifiers.Find(type);
        }

        [Pure]
        [ItemNotNull]
        internal static Optional<IPrettifier> FindPrettifier(Type type) {
            return Optional.Optional.FirstWithValue(
                type,
                FindExactPrettifier,
                FindGenericallyTypedPrettifier,
                FindInheritedPrettifier
            );
        }

        [Pure]
        [ItemNotNull]
        private static Optional<IPrettifier> FindInheritedPrettifier(Type type) {
            try {
                return new Optional<IPrettifier>(Prettifiers.First(it => it.PrettifierType.IsAssignableFrom(type)));
            }
            catch (InvalidOperationException) {
                return default;
            }
        }

        [Pure]
        [NotNull]
        public static string Prettify([CanBeNull] this object cinderella, [CanBeNull] PrettificationSettings settings = default) {
            if (cinderella == null) {
                return settings?.NullPlaceholder.Value.IsNotBlank() == true ? settings.NullPlaceholder.Value : DefaultNullPlaceholder;
            }

            return FindPrettifier(cinderella.GetType())
                .IfPresentOrElse(
                    it => it.PrettifySafely(cinderella, settings),
                    () => Convert.ToString(cinderella)
                );
        }
    }
}