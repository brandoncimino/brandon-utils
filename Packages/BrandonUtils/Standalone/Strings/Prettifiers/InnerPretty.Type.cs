using System;
using System.Linq;
using System.Runtime.CompilerServices;

using BrandonUtils.Standalone.Collections;
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
        public static string PrettifyType(this Type type, PrettificationSettings settings) {
            if (type.IsTupleType()) {
                return PrettifyTupleType(type);
            }

            // if the type is generic, we need to trim the `n and replace it with the generic type arguments
            return type.IsGenericTypeOrDefinition() ? PrettifyGenericType(type) : type.Name;
        }

        /**
         * <inheritdoc cref="PrettifyType(System.Type,BrandonUtils.Standalone.Strings.PrettificationSettings)"/>
         */
        internal static string PrettifyType(this Type type) {
            return PrettifyType(type, default);
        }

        private static string PrettifyGenericType(Type genericType) {
            if (!genericType.IsGenericType) {
                throw new ArgumentException($"{genericType} is not a generic type!", nameof(genericType));
            }

            return genericType.Name.Replace($"`{genericType.GenericTypeArguments.Length}", PrettifyGenericTypeArguments(genericType.GenericTypeArguments));
        }

        private static string PrettifyTupleType(Type tupleType) {
            var genArgs = tupleType.GetGenericArguments().Select(it => it.PrettifyType());
            return $"({genArgs.JoinString(", ")})";
        }

        private static string PrettifyGenericTypeArguments([CanBeNull] Type[] genericTypeArguments) {
            if (genericTypeArguments == null || genericTypeArguments.IsEmpty()) {
                return "";
            }

            return $"<{genericTypeArguments.Select(PrettifyType).JoinString(", ")}>";
        }
    }
}