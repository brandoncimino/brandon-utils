using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public static class AssertAll {
        public static void Of(params Action[] assertions) {
            var failures = new List<string>();
            foreach (var ass in assertions) {
                try {
                    ass.Invoke();
                }
                catch (Exception e) {
                    failures.Add($"{ass.Method.Name} failed!\n{e.Message}");
                }
            }

            if (failures.Any()) {
                Assert.Fail($"[{failures.Count}/{assertions.Length}] assertions failed:\n\n{failures.JoinLines()}");
            }
        }

        public static void Of<T>(T actual, params Constraint[] assertions) {
            var assActions = assertions.Select<Constraint, Action>(ass => () => Assert.That(actual, ass)).ToArray();
            Of(assActions);
        }
    }
}