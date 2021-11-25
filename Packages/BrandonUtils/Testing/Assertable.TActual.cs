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

        public string       Nickname { get; }
        public Func<string> Message  { get; }

        public Assertable(
            [CanBeNull] string                                                                 nickname,
            [NotNull]   ActualValueDelegate<TActual>                                           actual,
            [NotNull]   IResolveConstraint                                                     constraint,
            [CanBeNull] Func<string>                                                           message,
            [NotNull]   Action<ActualValueDelegate<TActual>, IResolveConstraint, Func<string>> trueResolver
        ) {
            Nickname = nickname ?? constraint.GetType().Prettify(AssertableExtensions.AssertablePrettificationSettings);
            Message  = message;

            try {
                trueResolver.Invoke(actual, constraint, message);
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
            [CanBeNull] string                                                 nickname,
            [NotNull]   TestDelegate                                           action,
            [NotNull]   IResolveConstraint                                     constraint,
            [CanBeNull] Func<string>                                           message,
            [NotNull]   Action<TestDelegate, IResolveConstraint, Func<string>> actionResolver
        ) {
            Nickname = nickname ?? action.Method.Name;
            Message  = message;

            try {
                actionResolver.Invoke(action, constraint, message);
                _excuse = default;
            }
            catch (SuccessException) {
                _excuse = default;
            }
            catch (Exception e) {
                _excuse = e;
            }
        }

        public override string ToString() {
            return this.FormatAssertable().JoinLines();
        }
    }
}