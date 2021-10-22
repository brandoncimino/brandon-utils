using System;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone {
    [PublicAPI]
    public static class ValidationExtensions {
        [NotNull]
        [ContractAnnotation("null => stop")]
        public static T MustNotBeNull<T>([CanBeNull] this T obj) {
            return obj ?? throw new ArgumentNullException($"Validated Parameter of type {typeof(T).Prettify()}");
        }
    }
}