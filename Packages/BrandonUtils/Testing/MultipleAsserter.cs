using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using NUnit.Framework;
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

        internal IList<(Action, Func<string>)>                                          Actions_AgainstAnything     { get; } = new List<(Action, Func<string>)>();
        internal IList<(Action<TActual>, Func<string>)>                                 Actions_AgainstActual       { get; } = new List<(Action<TActual>, Func<string>)>();
        internal IList<(IResolveConstraint, Func<string>)>                              Constraints_AgainstActual   { get; } = new List<(IResolveConstraint, Func<string>)>();
        internal IList<(object, IResolveConstraint, Func<string>)>                      Constraints_AgainstAnything { get; } = new List<(object, IResolveConstraint, Func<string>)>();
        internal IList<(ActualValueDelegate<object>, IResolveConstraint, Func<string>)> Constraints_AgainstDelegate { get; } = new List<(ActualValueDelegate<object>, IResolveConstraint, Func<string>)>();

        internal IList<IMultipleAsserter> Asserters { get; } = new List<IMultipleAsserter>();

        internal IList<(Func<TActual, object>, IResolveConstraint, Func<string>)> Constraints_AgainstTransformation { get; } = new List<(Func<TActual, object>, IResolveConstraint, Func<string>)>();

        protected abstract Action<string>                                                         ActionOnFailure          { get; }
        protected virtual  Action<string>                                                         ActionOnSuccess          { get; } = Console.WriteLine;
        protected abstract Action<ActualValueDelegate<object>, IResolveConstraint, Func<string>>  TrueResolver             { get; }
        protected abstract Action<ActualValueDelegate<TActual>, IResolveConstraint, Func<string>> TrueTypeResolver         { get; }
        protected abstract Action<TestDelegate, IResolveConstraint, Func<string>>                 ActionConstraintResolver { get; }
        // protected abstract Action<TActual, IResolveConstraint>                                    ConstraintResolver         { get; }
        // protected abstract Action<ActualValueDelegate<TActual>, IResolveConstraint>               ActualConstraintResolver   { get; }
        // protected abstract Action<ActualValueDelegate<object>, IResolveConstraint>                AnythingConstraintResolver { get; }
        // protected abstract Action<object, IResolveConstraint>                                     ObjectConstraintResolver   { get; }

        private Optional<Exception> ShortCircuitException;

        protected MultipleAsserter() { }

        protected MultipleAsserter(TActual actual) : this(() => actual) { }

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
        public TSelf And([CanBeNull] Action action, [CanBeNull] Func<string> message = default) {
            Actions_AgainstAnything.AddNonNull((action, message));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] Action action, string message) => And(action, () => message);

        [MustUseReturnValue]
        public TSelf And([CanBeNull, ItemCanBeNull] IEnumerable<Action> actions) {
            actions?.ForEach(it => _ = And((it, default)));
            return Self;
        }

        #endregion

        #region Actions_AgainstActual

        private void Add_Action_AgainstActual([CanBeNull] Action<TActual> action, Func<string> message) {
            Actions_AgainstActual.Add((action, message));
        }

        private void Add_Action_AgainstActual([CanBeNull] Action<TActual> action) => Add_Action_AgainstActual(action, default);

        [MustUseReturnValue]
        public TSelf And([CanBeNull] Action<TActual> action, Func<string> message = default) {
            Add_Action_AgainstActual(action, message);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] Action<TActual> action, string message) => And(action, () => message);

        [MustUseReturnValue]
        public TSelf And([CanBeNull] [ItemCanBeNull] IEnumerable<Action<TActual>> actions) {
            actions?.ForEach(Add_Action_AgainstActual);
            return Self;
        }

        #endregion

        #region Constraints_AgainstActual

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IResolveConstraint constraint, Func<string> message = default) {
            Constraints_AgainstActual.AddNonNull((constraint, message));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IResolveConstraint constraint, string message) => And(constraint, () => message);

        [MustUseReturnValue]
        public TSelf And([CanBeNull, ItemCanBeNull] IEnumerable<IResolveConstraint> constraints) {
            constraints?.ForEach(it => _ = And(it));
            return Self;
        }

        #endregion

        #region Constraints_AgainstAnything

        [MustUseReturnValue]
        public TSelf And((object, IResolveConstraint, Func<string>) constraint) {
            Constraints_AgainstAnything.Add(constraint);
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And((object, IResolveConstraint) constraint) => And((constraint.Item1, constraint.Item2, default));

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IEnumerable<(object, IResolveConstraint)> constraints) {
            constraints?.ForEach(it => _ = And((it.Item1, it.Item2, default)));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And([CanBeNull] object actual, [CanBeNull] IResolveConstraint constraint, Func<string> message = default) => And((actual, constraint, message));

        [MustUseReturnValue]
        public TSelf And([CanBeNull] object actual, [CanBeNull] IResolveConstraint constraint, string message) => And(actual, constraint, () => message);

        #endregion

        #region Constraints_AgainstDelegate

        [MustUseReturnValue]
        public TSelf And(ActualValueDelegate<object> testDelegate, IResolveConstraint constraint, Func<string> message = default) {
            Constraints_AgainstDelegate.AddNonNull((testDelegate, constraint, message));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(ActualValueDelegate<object> testDelegate, IResolveConstraint constraint, string message) => And(testDelegate, constraint, () => message);

        #endregion

        #region Constraints_AgainstTransformation

        [MustUseReturnValue]
        public TSelf And(Func<TActual, object> actualTransformation, IResolveConstraint constraint, Func<string> message = default) {
            Constraints_AgainstTransformation.Add((actualTransformation, constraint, message));
            return Self;
        }

        [MustUseReturnValue]
        public TSelf And(Func<TActual, object> actualTransformation, IResolveConstraint constraint, string message) => And(actualTransformation, constraint, () => message);

        [MustUseReturnValue]
        public TSelf And([CanBeNull] IEnumerable<(Func<TActual, object>, IResolveConstraint)> constraints) {
            constraints?.ForEach(it => _ = And(it.Item1, it.Item2));
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

        private IAssertable Test_Action_AgainstAnything((Action action, Func<string> nickname) ass) => new Assertable(
            ass.nickname,
            new TestDelegate(ass.action),
            Throws.Nothing,
            default,
            ActionConstraintResolver
        );

        private IAssertable Test_Action_AgainstActual((Action<TActual> action, Func<string> nickname) ass) {
            var actual = Actual.OrElseThrow(ActualIsEmptyException($"Could not execute the {ass.GetType().Prettify()} {ass.action.Method.Name}"));
            return new Assertable(
                ass.nickname,
                () => ass.action.Invoke(actual.Invoke()),
                Throws.Nothing,
                default,
                ActionConstraintResolver
            );
        }

        private IAssertable Test_Constraint_AgainstActual((IResolveConstraint constraint, Func<string> message) againstActual) {
            var actualValueDelegate = Actual.OrElseThrow(ActualIsEmptyException(againstActual.Item1));
            return Assertable.Assert(
                default,
                actualValueDelegate,
                againstActual.constraint,
                againstActual.message,
                TrueTypeResolver
            );
        }

        private IAssertable Test_Constraint_AgainstAnything((object, IResolveConstraint, Func<string>) constraint_againstAnything) {
            var (target, resolveConstraint, message) = constraint_againstAnything;
            return new Assertable(default, () => target, resolveConstraint, message, TrueResolver);
        }

        private IAssertable Test_Constraint_AgainstDelegate((ActualValueDelegate<object> tDelegate, IResolveConstraint constraint, Func<string>) constraint_againstDelegate) {
            var (tDelegate, constraint, message) = constraint_againstDelegate;
            return new Assertable(default, tDelegate, constraint, message, TrueResolver);
        }

        private IAssertable Test_Constraint_AgainstTransformation((Func<TActual, object> transformation, IResolveConstraint constraint, Func<string>) constraint_againstTransformation) {
            var (transformation, constraint, message) = constraint_againstTransformation;
            return new Assertable(
                default,
                () => transformation.Invoke(Actual.OrElseThrow(ActualIsEmptyException(constraint)).Invoke()),
                constraint,
                message,
                TrueResolver
            );
        }

        private IAssertable Test_Asserter(IMultipleAsserter asserter) {
            return Test_Action_AgainstAnything((asserter.Invoke, default));
        }

        #endregion

        #region Validations / Exceptions

        private ActualValueDelegate<TActual> MustGetActualValue(string message) {
            return Actual.OrElseThrow(ActualIsEmptyException(message));
        }

        private Func<InvalidOperationException> ActualIsEmptyException(IResolveConstraint constraint) {
            return ActualIsEmptyException($"Could not convert the {constraint.GetType().Name} to an {nameof(Action)}");
        }

        private Func<InvalidOperationException> ActualIsEmptyException(string message) {
            return () => new InvalidOperationException($"{message}: this {GetType().Prettify()} doesn't have {nameof(Actual)} value!");
        }

        #endregion

        private IEnumerable<IAssertable> TestEverything() {
            //TODO: Create dedicated classes for each of this constraint/action types; and/or a clean interface for them
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

            var countString = failures.IsNotEmpty() ? $"[{failures.Count}/{testResults.Count()}]" : $"All {testResults.Count()}";
            Console.WriteLine($"ACTUAL:{Actual.Prettify()}");
            var againstString = Actual.Select(it => it.Prettify(prettySettings))
                                      .Select(it => it.Truncate(prettySettings.LineLengthLimit))
                                      .Select(it => $" against {it}")
                                      .OrElse("");

            var summary = failures.IsNotEmpty()
                              ? $"💔 {countString} assertions{againstString} failed:"
                              : $"🎊 {countString} assertions{againstString} passed!";

            return new List<string>()
                   .Append(summary)
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
            if (valediction == null) {
                throw new ArgumentNullException(nameof(valediction));
            }

            valediction.Invoke(FormatMultipleAssertionMessage(assertables));
        }
    }
}