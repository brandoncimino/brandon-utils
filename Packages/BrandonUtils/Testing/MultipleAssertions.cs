using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Exceptions;
using BrandonUtils.Standalone.Strings;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    internal static class MultipleAssertions {
        internal static void ExecuteMultipleAssertions(string heading, Action[] assertions, Action<string> actionOnFailure) {
            var failures = new List<string>();
            foreach (var ass in assertions) {
                try {
                    ass.Invoke();
                }
                catch (Exception e) {
                    var stackTraceFilter = new StringFilter()
                                           .Matching(@"^\s*(in|at) NUnit")
                                           .Matching($@"^\s*(in|at) {typeof(AssertAll)}");
                    failures.Add($"{ass.Method.Name} failed!\n{e.Message}\n{e.FilteredStackTrace(stackTraceFilter).TruncateLines(10).JoinLines()}");
                }
            }

            if (failures.Any()) {
                var msg = formatHeading(heading, assertions, failures);

                actionOnFailure(msg);
            }
        }

        private static string formatHeading(string heading, Action[] assertions, List<string> failures) {
            var msg = $"[{failures.Count}/{assertions.Length}] assertions failed:\n\n{failures.JoinLines()}";
            if (!string.IsNullOrEmpty(heading)) {
                msg = heading + "\n" + msg;
            }

            return msg;
        }

        /// <summary>
        /// Takes an <see cref="IResolveConstraint"/> and builds a <b>parameterless <see cref="Action"/></b> out of it.
        ///
        /// The resulting action will perform some kind of "assertion" method (<paramref name="constraintResolutionAction"/>) against <paramref name="actual"/>, using the <paramref name="constraint"/>.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="constraint"></param>
        /// <param name="constraintResolutionAction">the method that applies the <paramref name="constraint"/> to the <paramref name="actual"/>, e.g. <see cref="Assert.That(TestDelegate,IResolveConstraint)"/></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static Action ConstraintToAction<T>(T actual, IResolveConstraint constraint, Action<T, IResolveConstraint> constraintResolutionAction) {
            return () => constraintResolutionAction.Invoke(actual, constraint);
        }
    }
}