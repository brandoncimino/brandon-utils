using System;
using System.Collections.Generic;

using BrandonUtils.Standalone.Chronic;
using BrandonUtils.Standalone.Enums;

using JetBrains.Annotations;

using NUnit.Framework.Constraints;

using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace BrandonUtils.Testing {
    [PublicAPI]
    public static class TestUtils {
        public const           double   ApproximationThreshold       = 0.001;
        public const           double   ApproximationThreshold_Loose = 0.005;
        public const           long     ApproximationTickThreshold   = (long)(TimeSpan.TicksPerSecond * ApproximationThreshold_Loose);
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

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, object expectedValue, object threshold, Clusivity clusivity = Clusivity.Inclusive) {
            return constraintExpression.Append(new ApproximationConstraint(expectedValue, threshold, clusivity)) as ApproximationConstraint;
        }

        public static ApproximationConstraint CloseTo(this ConstraintExpression constraintExpression, object expectedValue, object threshold) {
            return constraintExpression.Approximately(expectedValue, threshold);
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, object expectedValue) {
            return constraintExpression.Append(new ApproximationConstraint(expectedValue)) as ApproximationConstraint;
        }

        public static ApproximationConstraint CloseTo(this ConstraintExpression constraintExpression, object expectedValue) {
            return constraintExpression.Approximately(expectedValue);
        }

        public static ApproximationConstraint Approximately(this ConstraintExpression constraintExpression, DateTime expectedValue, TimeSpan threshold) {
            return constraintExpression.Append(new ApproximationConstraint(expectedValue, threshold)) as ApproximationConstraint;
        }

        public static ApproximationConstraint CloseTo(this ConstraintExpression constraintExpression, DateTime expectedValue, TimeSpan threshold) {
            return constraintExpression.Approximately(expectedValue, threshold);
        }

        public static WaitForSeconds WaitFor(TimeSpan timeSpan, double multiplier = 1) {
            return new WaitForSeconds((float)timeSpan.Multiply(multiplier).TotalSeconds);
        }

        public static WaitForSecondsRealtime WaitForRealtime(TimeSpan timeSpan, double multiplier = 1) {
            return new WaitForSecondsRealtime((float)timeSpan.Multiply(multiplier).TotalSeconds);
        }
    }
}