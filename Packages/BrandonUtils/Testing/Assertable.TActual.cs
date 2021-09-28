using System;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    /// <summary>
    /// A variation of <see cref="Assertable"/> that applies an <see cref="IResolveConstraint"/> to a <typeparamref name="TActual"/> value.
    /// </summary>
    /// <typeparam name="TActual">the type of the value being passed to the <see cref="IResolveConstraint"/></typeparam>
    public readonly struct Assertable<TActual> : IAssertable {
        private readonly Exception _excuse;
        public           Exception Excuse => _excuse ?? throw new InvalidOperationException($"Could not retrieve the {nameof(Excuse)} from the {this.GetType().Name} because {nameof(Failed)} = {Failed}!");
        public           bool      Failed => _excuse != null;

        public string Nickname { get; }

        public Assertable(
            [CanBeNull] TActual                             actual,
            [NotNull]   IResolveConstraint                  constraint,
            [NotNull]   Action<TActual, IResolveConstraint> constraintResolutionAction
        ) {
            Nickname = constraint.GetType().Prettify(AssertableExtensions.AssertablePrettificationSettings);
            try {
                constraintResolutionAction.Invoke(actual, constraint);
                _excuse = default;
            }
            catch (SuccessException) {
                _excuse = default;
            }
            catch (Exception e) {
                _excuse = e;
            }
        }

        public Assertable(
            [NotNull] ActualValueDelegate<TActual>                             actualValueDelegate,
            [NotNull] IResolveConstraint                                       constraint,
            [NotNull] Action<ActualValueDelegate<TActual>, IResolveConstraint> delegateConstraintResolutionAction
        ) {
            Nickname = constraint.GetType().Prettify();
            try {
                delegateConstraintResolutionAction.Invoke(actualValueDelegate, constraint);
                _excuse = default;
            }
            catch (SuccessException) {
                _excuse = default;
            }
            catch (Exception e) {
                _excuse = e;
            }
        }

        public Assertable(
            [CanBeNull] TActual         actual,
            [NotNull]   Action<TActual> assertion
        ) {
            Nickname = assertion.Method.Name;

            try {
                assertion.Invoke(actual);
                _excuse = default;
            }
            catch (SuccessException) {
                _excuse = default;
            }
            catch (Exception e) {
                _excuse = e;
            }
        }

        public Assertable(
            [NotNull] ActualValueDelegate<TActual> actualValueDelegate,
            [NotNull] Action<TActual>              assertion
        ) {
            Nickname = assertion.Method.Name;

            try {
                assertion.Invoke(GetActualValue(actualValueDelegate));
                _excuse = default;
            }
            catch (SuccessException) {
                _excuse = default;
            }
            catch (Exception e) {
                _excuse = e;
            }
        }

        private static TActual GetActualValue(ActualValueDelegate<TActual> actualValueDelegate) {
            try {
                return actualValueDelegate.Invoke();
            }
            catch (Exception e) {
                Assert.Fail($"Error when getting the result of the {nameof(actualValueDelegate)}!", e);
                throw;
            }
        }

        public override string ToString() {
            return this.FormatAssertable().JoinLines();
        }
    }
}