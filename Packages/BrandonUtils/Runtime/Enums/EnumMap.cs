using System;

namespace Packages.BrandonUtils.Runtime.Enums {
    /// <summary>
    /// I plan for this to be a special collection that will enforce that all values of <typeparamref name="TEnum"/> be assigned values of <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class EnumMap<TEnum, TValue> where TEnum : Enum {
        public EnumMap() {
            throw new NotImplementedException();
        }
    }
}