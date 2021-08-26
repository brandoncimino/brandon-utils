using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    [PublicAPI]
    public abstract class MultipleAsserter<TSelf, TActual> where TSelf : MultipleAsserter<TSelf, TActual>, new() {
        /// <summary>
        /// The actual value being asserted against (if there is one)
        /// </summary>
        public Optional<TActual> Actual;

        [CanBeNull]
        public Func<string> HeadingSupplier { get; set; }

        [ItemNotNull]
        public IList<Action> Actions { get; } = new List<Action>();

        [ItemNotNull]
        public IList<IResolveConstraint> Constraints { get; } = new List<IResolveConstraint>();

        protected abstract Action<string>                      ActionOnFailure    { get; }
        protected abstract Action<TActual, IResolveConstraint> ConstraintResolver { get; }

        private Optional<Exception> ShortCircuitException;

        protected MultipleAsserter() { }

        protected MultipleAsserter(TActual actual) {
            Actual = actual;
        }

        private TSelf Self => this as TSelf;

        public TSelf Against([CanBeNull] TActual actual) {
            Actual = actual;
            return Self;
        }

        public TSelf And([CanBeNull] Action action) {
            Actions.Add(action);
            return Self;
        }

        public TSelf And([CanBeNull] [ItemCanBeNull] IEnumerable<Action> actions) {
            Actions.AddNonNull(actions);
            return Self;
        }

        public TSelf And([CanBeNull] IResolveConstraint constraint) {
            Constraints.AppendNonNull(constraint);
            return Self;
        }

        public TSelf And([CanBeNull] [ItemCanBeNull] IEnumerable<IResolveConstraint> constraints) {
            Constraints.AddNonNull(constraints);
            return Self;
        }

        public TSelf WithHeading([NotNull] Func<string> headingSupplier) {
            HeadingSupplier = headingSupplier;
            return Self;
        }

        public TSelf WithHeading([CanBeNull] string heading) {
            HeadingSupplier = () => heading;
            return Self;
        }

        private static IAssertable TestAction([NotNull] Action action) {
            return new Assertable(action);
        }

        private IAssertable TestConstraint([NotNull] IResolveConstraint constraint) {
            if (Actual.IsEmpty()) {
                throw new InvalidOperationException($"Could not convert the {constraint.GetType().Name} to an {nameof(Action)} because this {GetType().Name} doesn't have an {nameof(Actual)} value!");
            }

            return new Assertable<TActual>(Actual.Value, constraint, ConstraintResolver);
        }

        private IEnumerable<IAssertable> TestEverything() {
            var actionFailures     = Actions.Select(TestAction);
            var constraintFailures = Constraints.Select(TestConstraint);
            return actionFailures.Concat(constraintFailures);
        }

        public void ShortCircuit(Exception shortCircuitException) {
            ShortCircuitException = shortCircuitException;
            Execute();
        }

        #region formatting

        private static IEnumerable<string> FormatFailures([InstantHandle] IEnumerable<IAssertable> testResults) {
            testResults = testResults.ToList();
            var failures = testResults.Where(it => it.Failed).ToList();
            return new List<string>()
                   .Append($"[{failures.Count}/{testResults.Count()}] assertions failed:")
                   .Concat(testResults.SelectMany(AssertableExtensions.FormatAssertable));
        }

        private string FormatMultipleAssertionMessage(IEnumerable<IAssertable> failures) {
            return new List<string>()
                   .Concat(FormatHeading())
                   .Concat(FormatShortCircuitException())
                   .Concat(FormatFailures(failures))
                   .ToStringLines()
                   .JoinLines();
        }

        /// <summary>
        /// Returns either the result of <see cref="HeadingSupplier"/> or an empty <see cref="IEnumerable{T}"/> of strings.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> FormatHeading() {
            return HeadingSupplier != null ? new[] { HeadingSupplier.Invoke() } : new string[] { };
        }

        private Optional<string> FormatShortCircuitException() {
            return ShortCircuitException.Select(it => $"Something caused this {GetType().Name} to be unable to execute all of the assertions that it wanted to:\n{it.Message}\n{it.StackTrace}");
        }

        #endregion

        public void Execute() {
            var failures = TestEverything().ToList();
            if (failures.Any(it => it.Failed)) {
                ActionOnFailure.Invoke(FormatMultipleAssertionMessage(failures));
            }
        }
    }
}