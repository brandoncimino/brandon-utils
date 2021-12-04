using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Enums {
    /// <summary>
    /// Contains factory methods for building <see cref="EnumSet{T}"/>s.
    /// </summary>
    [PublicAPI]
    public static class EnumSet {
        [NotNull]
        public static EnumSet<T> Of<T>([NotNull] params T[] enumValues) where T : struct, Enum {
            return new EnumSet<T>(enumValues);
        }

        public static EnumSet<T> Inverted<T>([NotNull] this ISet<T> original) where T : struct, Enum {
            return new EnumSet<T>(original);
        }

        public static bool IsEnumSet<T>(this ISet<T> set) where T : struct, Enum {
            return true;
        }

        [NotNull]
        public static EnumSet<T> Of<T>([NotNull] IEnumerable<T> values) where T : struct, Enum {
            return new EnumSet<T>(values);
        }

        [NotNull]
        public static EnumSet<T> OfAllValues<T>() where T : struct, Enum {
            return new EnumSet<T>(BEnum.GetValues<T>());
        }

        [NotNull]
        public static EnumSet<T> ToEnumSet<T>([NotNull] this IEnumerable<T> source) where T : struct, Enum {
            return new EnumSet<T>(source);
        }

        [NotNull]
        public static EnumSet<T> Of<T>([NotNull] params IEnumerable<T>[] sets) where T : struct, Enum {
            return new EnumSet<T>(sets);
        }
    }
}