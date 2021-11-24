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
    public abstract class MultipleAsserter<TSelf, TActual> : IMultipleAsserter where TSelf : MultipleAsserter<TSelf, TActual>, new() {
        private const string HeadingIcon = "🧪";

        /// <summary>
        /// The actual value being asserted against (if there is one)
        /// </summary>
        private Optional<ActualValueDelegate<TActual>> Actual;

        [CanBeNull] public Func<string> HeadingSupplier { get; set; }

        public int Indent { get; set; }

        [ItemNotNull]
        internal IList<Action> Actions_AgainstAnything { get; } = new List<Action>();

        [ItemNotNull]
        internal IList<Action<TActual>> Actions_AgainstActual { get; } = new List<Action<TActual>>();

        [ItemNotNull]
        internal IList<IResolveConstraint> Constraints_AgainstActual { get; } = new List<IResolveConstraint>();

        internal IList<(object, IResolveConstraint)>                      Constraints_AgainstAnything { get; } = new List<(object, IResolveConstraint)>();
        internal IList<(ActualValueDelegate<object>, IResolveConstraint)> Constraints_AgainstDelegate { get; } = new List<(ActualValueDelegate<object>, IResolveConstraint)>();

        internal IList<IMultipleAsserter> Asserters { get; } = new List<IMultipleAsserter>();

        internal IList<(Func<TActual, object>, IResolveConstraint)> Constraints_AgainstTransformation { get; } = new List<(Func<TActual, object>, IResolveConstraint)>();

        protected abstract Action<string>                                          ActionOnFailure            { get; }
        protected virtual  Action<string>                                          ActionOnSuccess            { get; } = Console.WriteLine;
        protected abstract Action<TActual, IResolveConstraint>                     ConstraintResolver         { get; }
        protected abstract Action<ActualValueDelegate<object>, IResolveConstraint> DelegateConstraintResolver { get; }
        protected abstract Action<object, IResolveConstraint>                      ObjectConstraintResolver   { get; }

        private Optional<Exception> ShortCircuitException;

        protected MultipleAsserter() { }

        protected MultipleAsserter(TActual actual) : this(() => actual) {
            Actual = new Optional<ActualValueDelegate<TActual>>(() => actual);
        }

        protected MultipleAsserter(ActualValueDelegate<TActual> actualValueDelegate) {
            Actual = actualValueDelegate;
        }

        private TSelf Self => this as TSelf;

        #region Builder

        [MustUseReturnValue]
        public TSelf Against([CanBeNull] TActual actual) {
            Actual = new Optional<ActualValueDelegate<TActual>>(() => actual);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf Against([NotNull] ActualValueDelegate<TActual> actualValueDelegate) {
            Actual = actualValueDelegate;
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

        #region Constraints_AgainstDelegate

        [MustUseReturnValue]
        public TSelf And(ActualValueDelegate<object> testDelegate, IResolveConstraint constraint) {
            Constraints_AgainstDelegate.AddNonNull((testDelegate, constraint));
            return Self;
        }

        #endregion

        #region Constraints_AgainstTransformation

        [MustUseReturnValue]
        public TSelf And(Func<TActual, object> actualTransformation, IResolveConstraint constraint) {
            return And((actualTransformation, constraint));
        }

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

        #region Asserters

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IMultipleAsserter asserter) {
            Asserters.AddNonNull(asserter);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] [ItemCanBeNull] IEnumerable<IMultipleAsserter> asserters) {
            Asserters.AddNonNull(asserters);
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

        #region With Indent

        [MustUseReturnValue]
        public TSelf WithIndent(int indent) {
            Indent = indent;
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
            var actualValue = MustGetActualValue($"Could not execute the {action.GetType().Prettify()} {action.Method.Name}");
            return new Assertable<TActual>(actualValue, action);
        }

        private IAssertable Test_Constraint_AgainstActual([NotNull] IResolveConstraint constraint) {
            var actualValue = MustGetActualValue(constraint);
            return new Assertable<TActual>(actualValue, constraint, ConstraintResolver);
        }

        private IAssertable Test_Constraint_AgainstAnything((object, IResolveConstraint) constraint_againstAnything) {
            var (actual, resolveConstraint) = constraint_againstAnything;
            return new Assertable<object>(actual, resolveConstraint, ObjectConstraintResolver);
        }

        private IAssertable Test_Constraint_AgainstDelegate((ActualValueDelegate<object>, IResolveConstraint) constraint_againstDelegate) {
            var (tDelegate, constraint) = constraint_againstDelegate;
            return new Assertable<object>(tDelegate, constraint, DelegateConstraintResolver);
        }

        private IAssertable Test_Constraint_AgainstTransformation((Func<TActual, object>, IResolveConstraint) constraintAgainstTransformation) {
            var (transformation, constraint) = constraintAgainstTransformation;
            var actualValue = MustGetActualValue(constraint);
            return new Assertable<object>(() => transformation.Invoke(actualValue), constraint, DelegateConstraintResolver);
        }

        private IAssertable Test_Asserter(IMultipleAsserter asserter) {
            return Test_Action_AgainstAnything(asserter.Invoke);
        }

        #endregion

        #region Validations / Exceptions

        private TActual MustGetActualValue(string message) {
            if (Actual.IsEmpty()) {
                throw ActualIsEmptyException(message);
            }

            return Actual.Value.Invoke();
        }

        private TActual MustGetActualValue(IResolveConstraint constraint) {
            if (Actual.IsEmpty()) {
                throw ActualIsEmptyException(constraint);
            }

            return Actual.Value.Invoke();
        }

        private InvalidOperationException ActualIsEmptyException(IResolveConstraint constraint) {
            return ActualIsEmptyException($"Could not convert the {constraint.GetType().Name} to an {nameof(Action)}");
        }

        private InvalidOperationException ActualIsEmptyException(string message) {
            return new InvalidOperationException($"{message}: this {GetType().Prettify()} doesn't have {nameof(Actual)} value!");
        }

        #endregion

        private IEnumerable<IAssertable> TestEverything() {
            return Actions_AgainstAnything.Select(Test_Action_AgainstAnything)
                                          .Concat(Actions_AgainstActual.Select(Test_Action_AgainstActual))
                                          .Concat(Constraints_AgainstActual.Select(Test_Constraint_AgainstActual))
                                          .Concat(Constraints_AgainstAnything.Select(Test_Constraint_AgainstAnything))
                                          .Concat(Constraints_AgainstTransformation.Select(Test_Constraint_AgainstTransformation))
                                          .Concat(Constraints_AgainstDelegate.Select(Test_Constraint_AgainstDelegate))
                                          .Concat(Asserters.Select(Test_Asserter));
        }

        [ContractAnnotation("=> stop")]
        public void ShortCircuit(Exception shortCircuitException) {
            ShortCircuitException = shortCircuitException;
            Invoke();
        }

        #region formatting

        [NotNull, ItemNotNull]
        private IEnumerable<string> FormatFailures([InstantHandle] IEnumerable<IAssertable> testResults) {
            testResults = testResults.ToList();
            var failures = testResults.Where(it => it.Failed).ToList();

            var prettySettings = new PrettificationSettings() {
                PreferredLineStyle = { Value = LineStyle.Single },
                LineLengthLimit    = { Value = 20 },
                TypeLabelStyle     = { Value = TypeNameStyle.Full }
            };

            var countString = $"[{failures.Count}/{testResults.Count()}]";
            var againstString = Actual.Select(it => it.Prettify(prettySettings))
                                      .Select(it => it.Truncate(prettySettings.LineLengthLimit))
                                      .Select(it => $" against {it}")
                                      .OrElse("");

            return new List<string>()
                   .Append($"{countString} assertions failed{againstString}:")
                   .Concat(testResults.SelectMany(it => it.FormatAssertable(Indent + 1)));
        }

        [NotNull]
        private string FormatMultipleAssertionMessage(IEnumerable<IAssertable> failures) {
            return new List<string>()
                   .Concat(FormatHeading())
                   .Concat(FormatShortCircuitException())
                   .Concat(FormatFailures(failures))
                   .ToStringLines()
                   .Indent(Indent)
                   .JoinLines();
        }

        /// <summary>
        /// Returns either the result of <see cref="HeadingSupplier"/> or an empty <see cref="IEnumerable{T}"/> of strings.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        private IEnumerable<string> FormatHeading() {
            return HeadingSupplier != null ? new[] { HeadingSupplier.Invoke() } : new string[] { };
        }

        private Optional<string> FormatShortCircuitException() {
            return ShortCircuitException.Select(it => $"Something caused this {GetType().Name} to be unable to execute all of the assertions that it wanted to:\n{it.Message}\n{it.StackTrace}");
        }

        #endregion

        public void Invoke() {
            var assertables = TestEverything().ToList();
            var valediction = assertables.Any(it => it.Failed) ? ActionOnFailure : ActionOnSuccess;
            valediction.Invoke(FormatMultipleAssertionMessage(assertables));
        }
    }
}