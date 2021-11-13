using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;
using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Enums {
    [PublicAPI]
    public static class BEnum {
        /// <typeparam name="T">an <see cref="Enum"/> type</typeparam>
        /// <returns>an array containing the <see cref="Type.GetEnumValues"/> of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">if <typeparamref name="T"/> is not an <see cref="Enum"/> type</exception>
        public static T[] GetValues<T>() where T : Enum {
            var enumType = typeof(T);
            if (enumType.IsEnum) {
                return enumType.GetEnumValues().Cast<T>().ToArray();
            }

            throw new ArgumentException($"{enumType.PrettifyType(default)} is not an enum type!");
        }

        public static T[] GetValues<T>(Type enumType) where T : struct, Enum {
            if (typeof(T) != enumType) {
                throw new ArgumentException($"The {nameof(enumType)} {enumType.PrettifyType(default)} was not the same as the type argument {nameof(T)}!");
            }

            if (!enumType.IsEnum) {
                throw new ArgumentException($"{enumType.PrettifyType(default)} is not an enum type!");
            }

            return enumType.GetEnumValues().Cast<T>().ToArray();
        }

        /// <summary>
        /// Creates an <see cref="System.ComponentModel.InvalidEnumArgumentException"/> using generics to infer the enum's <see cref="Type"/>.
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="enumValue"></param>
        /// <param name="allowedValues"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NotNull]
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            [CanBeNull] string argumentName,
            T                  enumValue,
            [CanBeNull, InstantHandle]
            IEnumerable<T> allowedValues = default
        ) where T : struct, Enum {
            return new InvalidEnumArgumentException(argumentName, (int)(object)enumValue, typeof(T));
        }

        [NotNull]
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            [CanBeNull] string argumentName,
            T?                 enumValue
        ) where T : struct, Enum {
            return new InvalidEnumArgumentException(argumentName, -1, typeof(T));
        }

        #region Enum not in set

        [NotNull]
        private static string BuildEnumNotInSetMessage(
            [CanBeNull] string              paramName,
            [NotNull]   Type                enumType,
            [NotNull]   IEnumerable<object> checkedValues,
            [NotNull]   IEnumerable<object> allowedValues
        ) {
            var badValues = checkedValues.Except(allowedValues);

            var msg = $"{enumType.PrettifyType(default)} values {badValues.Prettify()} aren't among the allowed values!";

            var dic = new Dictionary<object, object>() {
                    ["Enum type"]      = enumType,
                    ["Parameter name"] = paramName,
                    ["Allowed values"] = allowedValues,
                    ["Checked values"] = checkedValues,
                    ["Bad values"]     = badValues
                }.SelectValues(it => it.Prettify())
                 .WhereValues(it => it.IsNotBlank());

            var prettyDic = dic.Prettify(HeaderStyle.None);

            return new object[] {
                msg,
                prettyDic.SplitLines().Indent(),
            }.JoinLines();
        }

        [NotNull]
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            [CanBeNull] string         argumentName,
            [NotNull]   IEnumerable<T> checkedValues,
            [NotNull]   IEnumerable<T> allowedValues
        ) where T : struct, Enum {
            return new InvalidEnumArgumentException(
                BuildEnumNotInSetMessage(
                    argumentName,
                    typeof(T),
                    checkedValues.Cast<object>(),
                    allowedValues.Cast<object>()
                )
            );
        }

        [NotNull]
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(
            [CanBeNull] string          argumentName,
            [NotNull]   IEnumerable<T?> checkedValues,
            [NotNull]   IEnumerable<T?> allowedValues
        ) where T : struct, Enum {
            return new InvalidEnumArgumentException(
                BuildEnumNotInSetMessage(
                    argumentName,
                    typeof(T?),
                    checkedValues.Cast<object>(),
                    allowedValues.Cast<object>()
                )
            );
        }

        #endregion
    }
}