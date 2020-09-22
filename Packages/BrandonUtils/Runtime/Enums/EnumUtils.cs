using System;

namespace Packages.BrandonUtils.Runtime.Enums {
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
    }
}