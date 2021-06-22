using System;

namespace BrandonUtils.Refreshing {
    public static class Per {
        /// <summary>
        /// Constructs a new <see cref="Frame{T}"/>.
        /// </summary>
        /// <param name="valueSupplier"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PerFrame<T> Frame<T>(Func<T> valueSupplier) where T : IEquatable<T> {
            return new PerFrame<T>(valueSupplier);
        }
    }
}