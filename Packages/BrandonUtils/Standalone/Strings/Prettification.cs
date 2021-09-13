using System;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Reflection;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Strings {
    [PublicAPI]
    public static class Prettification {
        private static readonly KeyedList<Type, IPrettifier> Prettifiers = PrettifierDatabase.GetDefaultPrettifiers();

        public static void RegisterPrettifier(IPrettifier prettifier) {
            Prettifiers.Add(prettifier);
        }

        public static IPrettifier UnregisterPrettifier(Type prettifierType) {
            return Prettifiers.Grab(prettifierType);
        }

        private static Optional<IPrettifier> FindGenericallyTypedPrettifier(Type type) {
            return type.IsGenericTypeOrDefinition() ? Prettifiers.FindFirst(it => GenericTypesMatch(it.PrettifierType, type)) : default;
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

        private static Type MakeGenericOfObjects(Type type) {
            if (!type.IsGenericTypeOrDefinition()) {
                throw new ArgumentException("Must be a generic type or definition!", nameof(type));
            }

            var genArgCount = type.GetGenericArguments().Length;
            return type.GetGenericTypeDefinition().MakeGenericType(genArgCount.Repeat(typeof(object)).ToArray());
        }

        internal static Optional<IPrettifier> FindExactPrettifier(Type type) {
            return Prettifiers.Find(type);
        }

        internal static Optional<IPrettifier> FindPrettifier(Type type) {
            return Optional.Optional.FirstWithValue(
                type,
                FindExactPrettifier,
                FindGenericallyTypedPrettifier,
                FindInheritedPrettifier
            );
        }

        private static Optional<IPrettifier> FindInheritedPrettifier(Type type) {
            try {
                return new Optional<IPrettifier>(Prettifiers.First(it => it.PrettifierType.IsAssignableFrom(type)));
            }
            catch (InvalidOperationException) {
                return default;
            }
        }

        public static string Prettify([CanBeNull] this object cinderella, PrettificationSettings settings = default) {
            if (cinderella == null) {
                return "";
            }

            return FindPrettifier(cinderella.GetType())
                .IfPresentOrElse(
                    it => it.PrettifySafely(cinderella, settings),
                    () => Convert.ToString(cinderella)
                );
        }
    }
}