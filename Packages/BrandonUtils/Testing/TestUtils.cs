using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Collections;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace BrandonUtils.Testing {
    public static class TestUtils {
        public const           double   ApproximationThreshold       = 0.001;
        public const           double   ApproximationThreshold_Loose = 0.005;
        public const           long     ApproximationTickThreshold   = (long) (TimeSpan.TicksPerSecond * ApproximationThreshold_Loose);
        public static readonly TimeSpan ApproximationTimeThreshold   = TimeSpan.FromTicks(ApproximationTickThreshold);

        /// <summary>
        /// Assert that <paramref name="expectedList"/> and <see cref="actualList"/> match <b>exactly</b>.
        ///
        /// TODO: Now that I know more about NUnit, can this be done with stuff like <see cref="NUnit.Framework.Constraints.CollectionConstraint"/>? If not, can this be converted to use the <see cref="ConstraintExpression"/> system?
        /// </summary>
        /// <param name="expectedList"></param>
        /// <param name="actualList"></param>
        /// <typeparam name="T"></typeparam>
        [Obsolete("Please use " + nameof(NUnit.Framework.Assert.AreEqual) + " instead.")]
        public static void AreEqual<T>(IList<T> expectedList, IList<T> actualList) {
            UnityEngine.Assertions.Assert.AreEqual(expectedList.Count, actualList.Count, "The lists weren't the same length!");
            for (int i = 0; i < expectedList.Count; i++) {
                Debug.Log($"Comparing {expectedList[i]} == {actualList[i]}");
                UnityEngine.Assertions.Assert.AreEqual(expectedList[i], actualList[i], $"The lists differ at index [{i}]!");
            }
        }

        /// <summary>
        /// An extension method, intended to be called against <see cref="NUnit.Framework.Has"/>, to apply the <see cref="AllValuesConstraint"/>.
        /// </summary>
        /// <param name="constraintExpression"></param>
        /// <returns></returns>
        public static ConstraintExpression Values(this ConstraintExpression constraintExpression) {
            return constraintExpression.Append(new ValuesOperator());
        }

        public static ApproximationConstraint Approximately<T>(this ConstraintExpression constraintExpression, T expectedValue, T threshold) {
            // return (RangeConstraint) constraintExpression.Append(new RangeConstraint((dynamic) expectedValue - threshold, (dynamic) expectedValue + threshold));
            return (ApproximationConstraint) constraintExpression.Append(new ApproximationConstraint(expectedValue, threshold));
        }

        public static ApproximationConstraint Approximately<T>(this ConstraintExpression constraintExpression, T expectedValue) {
            // return Approximately(constraintExpression, expectedValue, (dynamic) expectedValue * ApproximationThreshold);
            throw new NotImplementedException("The usage of `(dynamic)` here is causing issues, I believe based on the .NET vs .NET Core vs .NET Framework version that other projects are targeting for their API compatibility. It should be removed if possible. Plus, coming back and looking at it...what does `dynamic` even do?!");
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, DateTime expectedValue, TimeSpan threshold) {
            return (ApproximationConstraint) constraintExpression.Append(new ApproximationConstraint(expectedValue, threshold));
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, DateTime expectedValue) {
            return constraintExpression.Approximately(expectedValue, ApproximationTimeThreshold);
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, TimeSpan expectedValue) {
            return constraintExpression.Approximately(expectedValue, ApproximationTimeThreshold);
        }

        public static WaitForSeconds WaitFor(TimeSpan timeSpan, double multiplier = 1) {
            return new WaitForSeconds((float) timeSpan.Multiply(multiplier).TotalSeconds);
        }

        public static WaitForSecondsRealtime WaitForRealtime(TimeSpan timeSpan, double multiplier = 1) {
            return new WaitForSecondsRealtime((float) timeSpan.Multiply(multiplier).TotalSeconds);
        }

        public static void AssertAll(params Action[] assertions) {
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
                Assert.Fail($"{failures.Count} / {assertions.Length} assertions failed:\n\n{failures.JoinLines()}");
            }
        }

        public static void AssertAll<T>(T actual, params Constraint[] assertions) {
            var assActions = assertions.Select<Constraint, Action>(ass => () => Assert.That(actual, ass)).ToArray();
            AssertAll(assActions);
        }
    }
}