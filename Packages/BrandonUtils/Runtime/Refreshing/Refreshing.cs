using System;

namespace BrandonUtils.Refreshing {
    public static class Refreshing {
        /// <summary>
        /// Constructs a new <see cref="PerFrame{T}"/>.
        /// </summary>
        /// <param name="valueSupplier"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PerFrame<T> PerFrame<T>(Func<T> valueSupplier) where T : IEquatable<T> {
            return new PerFrame<T>(valueSupplier);
        }
    }
}