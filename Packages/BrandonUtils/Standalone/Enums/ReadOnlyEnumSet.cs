using System;
using System.Collections.Generic;

using BrandonUtils.Standalone.Collections;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Enums {
    public class ReadOnlyEnumSet<T> : ReadOnlySet<T>, IEnumSet<T> where T : struct, Enum {
        public ReadOnlyEnumSet([NotNull] ISet<T>     realSet) : base(realSet) { }
        public ReadOnlyEnumSet([NotNull] IEnumSet<T> realSet) : base(realSet) { }

        public IEnumSet<T> Inverse() {
            var set1 = new HashSet<int>();
            var set2 = new HashSet<DayOfWeek>();

            set2.IsEnumSet();
            set2.Inverted();

            throw new NotImplementedException();
        }
    }
}