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

        private TActual GetActual() => Actual.Value;

        [CanBeNull] public Func<string> HeadingSupplier { get; set; }

        [ItemNotNull]
        internal IList<Action> Actions_AgainstAnything { get; } = new List<Action>();

        [ItemNotNull]
        internal IList<Action<TActual>> Actions_AgainstActual { get; } = new List<Action<TActual>>();

        [ItemNotNull]
        internal IList<IResolveConstraint> Constraints_AgainstActual { get; } = new List<IResolveConstraint>();

        internal IList<(object, IResolveConstraint)> Constraints_AgainstAnything { get; } = new List<(object, IResolveConstraint)>();

        internal IList<(Func<TActual, object>, IResolveConstraint)> Constraints_AgainstTransformation { get; } = new List<(Func<TActual, object>, IResolveConstraint)>();

        protected abstract Action<string>                      ActionOnFailure          { get; }
        protected abstract Action<TActual, IResolveConstraint> ConstraintResolver       { get; }
        protected abstract Action<object, IResolveConstraint>  ObjectConstraintResolver { get; }

        private Optional<Exception> ShortCircuitException;

        protected MultipleAsserter() { }

        protected MultipleAsserter(TActual actual) {
            Actual = actual;
        }

        private TSelf Self => this as TSelf;

        #region Builder

        [MustUseReturnValue]
        public TSelf Against([CanBeNull] TActual actual) {
            Actual = actual;
            return Self;
        }

        #region "And" Constraints

        #region Actions_AgainstAnything

        [MustUseReturnValue]
        public TSelf And([CanBeNull] Action action) {
            Actions_AgainstAnything.AddNonNull(action);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] [ItemCanBeNull] IEnumerable<Action> actions) {
            Actions_AgainstAnything.AddNonNull(actions);
            return Self;
        }

        #endregion

        #region Actions_AgainstActual

        [MustUseReturnValue]
        public TSelf And([CanBeNull] Action<TActual> action) {
            Actions_AgainstActual.AddNonNull(action);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] [ItemCanBeNull] IEnumerable<Action<TActual>> actions) {
            Actions_AgainstActual.AddNonNull(actions);
            return Self;
        }

        #endregion

        #region Constraints_AgainstActual

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IResolveConstraint constraint) {
            Constraints_AgainstActual.AddNonNull(constraint);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] [ItemCanBeNull] IEnumerable<IResolveConstraint> constraints) {
            Constraints_AgainstActual.AddNonNull(constraints);
            return Self;
        }

        #endregion

        #region Constraints_AgainstAnything

        [MustUseReturnValue]
        public TSelf And((object, IResolveConstraint) constraint) {
            Constraints_AgainstAnything.AddNonNull(constraint);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IEnumerable<(object, IResolveConstraint)> constraints) {
            Constraints_AgainstAnything.AddNonNull(constraints);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] object actual, [CanBeNull] IResolveConstraint constraint) {
            Constraints_AgainstAnything.AddNonNull((actual, constraint));
            return Self;
        }

        #endregion

        #region Constraints_AgainstTransformation

        [MustUseReturnValue]
        public TSelf And((Func<TActual, object>, IResolveConstraint) constraint) {
            Constraints_AgainstTransformation.AddNonNull(constraint);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IEnumerable<(Func<TActual, object>, IResolveConstraint)> constraints) {
            Constraints_AgainstTransformation.AddNonNull(constraints);
            return Self;
        }

        #endregion

        #endregion

        #region WithHeading

        [MustUseReturnValue]
        public TSelf WithHeading([NotNull] Func<string> headingSupplier) {
            HeadingSupplier = headingSupplier;
            return Self;
        }

        [MustUseReturnValue]
        public TSelf WithHeading([CanBeNull] string heading) {
            HeadingSupplier = () => heading;
            return Self;
        }

        #endregion

        #endregion

        #region Executing Test Assertions

        /// <summary>
        /// </summary>
        /// <remarks>
        /// This method returns <see cref="IAssertable"/> instead of <see cref="Assertable"/> to make it play nice with the typed <see cref="Assertable{TActual}"/>,
        /// which would be returned by <see cref="Test_Constraint_AgainstActual"/>.
        /// </remarks>
        /// <param name="action"></param>
        /// <returns></returns>
        private static IAssertable Test_Action_AgainstAnything([NotNull] Action action) {
            return new Assertable(action);
        }

        private IAssertable Test_Action_AgainstActual([NotNull] Action<TActual> action) {
            if (Actual.IsEmpty()) {
                throw ActualIsEmptyException($"Could not execute the {action.GetType().Prettify()} {action.Method.Name}");
            }

            return new Assertable<TActual>(Actual.Value, action);
        }

        private IAssertable Test_Constraint_AgainstActual([NotNull] IResolveConstraint constraint) {
            if (Actual.IsEmpty()) {
                throw ActualIsEmptyException(constraint);
            }

            return new Assertable<TActual>(Actual.Value, constraint, ConstraintResolver);
        }

        private IAssertable Test_Constraint_AgainstAnything([CanBeNull] object actual, [NotNull] IResolveConstraint constraint) {
            return new Assertable<object>(actual, constraint, ObjectConstraintResolver);
        }

        private IAssertable Test_Constraint_AgainstAnything((object, IResolveConstraint) constraint) {
            var (actual, resolveConstraint) = constraint;
            return Test_Constraint_AgainstAnything(actual, resolveConstraint);
        }

        private IAssertable Test_Constraint_AgainstTransformation((Func<TActual, object>, IResolveConstraint) constraint) {
            var (transformation, resolveConstraint) = constraint;
            if (Actual.IsEmpty()) {
                throw ActualIsEmptyException(resolveConstraint);
            }

            var transformed = Actual.Select(transformation.Invoke);
            return Test_Constraint_AgainstAnything(transformed, resolveConstraint);
        }

        #endregion

        private InvalidOperationException ActualIsEmptyException(IResolveConstraint constraint) {
            return ActualIsEmptyException($"Could not convert the {constraint.GetType().Name} to an {nameof(Action)}");
        }

        private InvalidOperationException ActualIsEmptyException(string message) {
            return new InvalidOperationException($"{message}: this {GetType().Prettify()} doesn't have {nameof(Actual)} value!");
        }

        private IEnumerable<IAssertable> TestEverything() {
            return Actions_AgainstAnything.Select(Test_Action_AgainstAnything)
                                          .Concat(Actions_AgainstActual.Select(Test_Action_AgainstActual))
                                          .Concat(Constraints_AgainstActual.Select(Test_Constraint_AgainstActual))
                                          .Concat(Constraints_AgainstAnything.Select(Test_Constraint_AgainstAnything))
                                          .Concat(Constraints_AgainstTransformation.Select(Test_Constraint_AgainstTransformation));
        }

        public void ShortCircuit(Exception shortCircuitException) {
            ShortCircuitException = shortCircuitException;
            Invoke();
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

        public void Invoke() {
            var failures = TestEverything().ToList();
            if (failures.Any(it => it.Failed)) {
                ActionOnFailure.Invoke(FormatMultipleAssertionMessage(failures));
            }
        }
    }
}