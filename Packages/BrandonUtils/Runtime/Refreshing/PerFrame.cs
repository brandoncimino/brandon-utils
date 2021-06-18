using System;

using BrandonUtils.Standalone.Refreshing;

using UnityEngine;

namespace BrandonUtils.Refreshing {
    public class PerFrame<TValue> : Refreshing<TValue, int> where TValue : IEquatable<TValue> {
        public PerFrame(Func<TValue> valueSupplier) : base(
            valueSupplier,
            () => Time.frameCount,
            (previous, current) => current != previous
        ) { }
    }
}