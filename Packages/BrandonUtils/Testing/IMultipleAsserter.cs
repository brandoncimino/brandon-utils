using System;

using JetBrains.Annotations;

namespace BrandonUtils.Testing {
    public interface IMultipleAsserter {
        void Invoke();

        [CanBeNull] Func<string> Heading { get; }

        int Indent { get; set; }
    }
}