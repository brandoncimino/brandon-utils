using System;
using System.Collections.Generic;

namespace BrandonUtils.Standalone.Enums {
    public static class EnumSet {
        public static EnumSet<T> Of<T>(params T[] enumValues) where T : struct, Enum {
            return new EnumSet<T>(enumValues);
        }

        public static EnumSet<T> Of<T>(IEnumerable<T> values) where T : struct, Enum {
            return new EnumSet<T>(values);
        }

        public static EnumSet<T> OfAllValues<T>() where T : struct, Enum {
            return new EnumSet<T>(BEnum.GetValues<T>());
        }

        public static EnumSet<T> ToEnumSet<T>(this IEnumerable<T> source) where T : struct, Enum {
            return new EnumSet<T>(source);
        }

        public static EnumSet<T> Of<T>(params IEnumerable<T>[] sets) where T : struct, Enum {
            return new EnumSet<T>(sets);
        }
    }
}