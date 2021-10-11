using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Collections {
    public static class DictionaryExtensions {
        [CanBeNull]
        public static TValue GetOrDefault<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dic,
            [NotNull]      TKey                      key,
            [CanBeNull]    TValue                    fallback = default
        ) {
            if (dic == null) {
                throw new ArgumentNullException(nameof(dic));
            }

            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            return dic.ContainsKey(key) ? dic[key] : fallback;
        }

        [CanBeNull]
        public static TValue GetOrDefault<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dic,
            [NotNull]      TKey                      key,
            [NotNull]      Func<TValue>              fallbackSupplier
        ) {
            if (dic == null) {
                throw new ArgumentNullException(nameof(dic));
            }

            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            if (fallbackSupplier == null) {
                throw new ArgumentNullException(nameof(fallbackSupplier));
            }

            return dic.ContainsKey(key) ? dic[key] : fallbackSupplier.Invoke();
        }
    }
}