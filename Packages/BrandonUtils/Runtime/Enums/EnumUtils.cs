using System;
using System.ComponentModel;

namespace BrandonUtils.Enums {
    public static class EnumUtils {
        public static T Step<T>(this T currentEnumValue, int step) where T : Enum {
            if (step < 0) {
                throw new IndexOutOfRangeException(
                    $"Wait; I don't think this will work for negative numbers like {step}..."
                );
            }

            int enumCount        = Enum.GetValues(typeof(T)).Length;
            int currentEnumIndex = (int) (object) currentEnumValue;
            int nextEnumIndex    = (currentEnumIndex + step) % enumCount;

            // ReSharper disable once PossibleInvalidCastException
            return (T) (object) nextEnumIndex;
        }

        public static T Next<T>(this T currentEnumValue) where T : Enum {
            return currentEnumValue.Step(1);
        }

        public static T Previous<T>(this T currentEnumValue) where T : Enum {
            return currentEnumValue.Step(-1);
        }

        /// <summary>
        /// Creates an <see cref="System.ComponentModel.InvalidEnumArgumentException"/> using generics to infer the enum's <see cref="Type"/>.
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="enumValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static InvalidEnumArgumentException InvalidEnumArgumentException<T>(string argumentName, T enumValue) where T : Enum {
            return new InvalidEnumArgumentException(argumentName, (int) (object) enumValue, typeof(T));
        }
    }
}