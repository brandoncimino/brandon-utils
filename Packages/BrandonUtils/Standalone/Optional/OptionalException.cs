using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    internal static class OptionalException {
        public static InvalidOperationException IsEmptyException<T>([NotNull] IOptional<T> self) {
            return new InvalidOperationException($"Unable to retrieve the {nameof(self.Value)} from the {self.GetType().Name} because it is empty!");
        }
    }
}