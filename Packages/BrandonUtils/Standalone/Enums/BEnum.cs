using System;
using System.ComponentModel;
using System.Linq;

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

            throw new ArgumentException($"{enumType.PrettifyType()} is not an enum type!");
        }

        public static T[] GetValues<T>(Type enumType) where T : struct, Enum {
            if (typeof(T) != enumType) {
                throw new ArgumentException($"The {nameof(enumType)} {enumType.PrettifyType()} was not the same as the type argument {nameof(T)}!");
            }

            if (!enumType.IsEnum) {
                throw new ArgumentException($"{enumType.PrettifyType()} is not an enum type!");
            }

            return enumType.GetEnumValues().Cast<T>().ToArray();
        }

        /// <summary>
        /// Creates an <see cref="System.ComponentModel.InvalidEnumArgumentException"/> using generics to infer the enum's <see cref="Type"/>.
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="enumValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(string argumentName, T enumValue) where T : Enum {
            return new InvalidEnumArgumentException(argumentName, (int)(object)enumValue, typeof(T));
        }
    }
}