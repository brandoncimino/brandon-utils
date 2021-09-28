using System;
using System.Linq;
using System.Runtime.CompilerServices;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Enums;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Reflection;

using JetBrains.Annotations;

[assembly: InternalsVisibleTo("BrandonUtils.Standalone.Strings")]

namespace BrandonUtils.Standalone.Strings.Prettifiers {
    internal static partial class InnerPretty {
        /// <summary>
        /// Prettifies a type, which may have different rules depending on whether it's a <see cref="ValueTuple{T1}"/>, <see cref="Type.IsGenericType"/>, etc.
        /// </summary>
        /// <param name="type">the <see cref="Type"/> to make pretty</param>
        /// <param name="settings">optional <see cref="PrettificationSettings"/></param>
        /// <returns>a pretty <see cref="string"/></returns>
        public static string PrettifyType([CanBeNull] this Type type, [CanBeNull] PrettificationSettings settings) {
            if (type == null) {
                return settings?.NullPlaceholder.GetValueOrDefault("");
            }

            if (type.IsTupleType()) {
                return PrettifyTupleType(type, settings);
            }

            // if the type is generic, we need to trim the `n and replace it with the generic type arguments
            return type.IsGenericTypeOrDefinition() ? PrettifyGenericType(type, settings) : type.Name;
        }

        /**
         * <inheritdoc cref="PrettifyType(System.Type,BrandonUtils.Standalone.Strings.PrettificationSettings)"/>
         */
        [NotNull]
        internal static string PrettifyType([CanBeNull] this Type type) {
            return PrettifyType(type, default);
        }

        private static string PrettifyGenericType([CanBeNull] Type genericType, [CanBeNull] PrettificationSettings settings = default) {
            if (genericType?.IsGenericType != true) {
                throw new ArgumentException($"{genericType} is not a generic type!", nameof(genericType));
            }

            return genericType.Name.Replace($"`{genericType.GenericTypeArguments.Length}", PrettifyGenericTypeArguments(genericType.GenericTypeArguments, settings));
        }

        private static string PrettifyTupleType(Type tupleType, [CanBeNull] PrettificationSettings settings) {
            var genArgs = tupleType.GetGenericArguments().Select(it => it.PrettifyType(settings));
            return $"({genArgs.JoinString(", ")})";
        }

        private static string PrettifyGenericTypeArguments([CanBeNull] Type[] genericTypeArguments, [CanBeNull] PrettificationSettings settings) {
            genericTypeArguments ??= Array.Empty<Type>();
            var stylizedArgs = StylizeGenericTypeArguments(genericTypeArguments, settings);
            return $"<{stylizedArgs}>";
        }

        private static string StylizeGenericTypeArguments([NotNull] [ItemCanBeNull] Type[] genericTypeArguments, [CanBeNull] PrettificationSettings settings) {
            settings ??= new PrettificationSettings();
            return settings.TypeLabelStyle.Value switch {
                TypeNameStyle.None  => "",
                TypeNameStyle.Full  => genericTypeArguments.Select(it => it.PrettifyType(settings)).JoinString(", "),
                TypeNameStyle.Short => genericTypeArguments.Select(_ => "").JoinString(","),
                _                   => throw BEnum.InvalidEnumArgumentException(nameof(settings.TypeLabelStyle.Value), settings.TypeLabelStyle.Value)
            };
        }

        internal static string WithTypeLabel([CanBeNull] this string thing, [NotNull] Type labelType, [CanBeNull] PrettificationSettings settings) {
            return $"{labelType.GetTypeLabel(settings)}{thing}";
        }
    }
}