using System;

using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Testing {
    public interface IMultipleAsserter {
        void Invoke();

        [CanBeNull] Func<string> Heading { get; }

        int Indent { get; set; }

        PrettificationSettings PrettificationSettings { get; }
    }
}