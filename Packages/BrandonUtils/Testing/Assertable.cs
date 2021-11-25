using System;
using System.Reflection;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

using NUnit.Framework;

namespace BrandonUtils.Testing {
    public interface IAssertable : IFailable {
        public string       Nickname { get; }
        public Func<string> Message  { get; }
    }

    /// <summary>
    /// A special implementation of <see cref="IFailable{TExcuse}"/> that handles the special case of <see cref="NUnit.Framework.SuccessException"/>.
    /// </summary>
    public readonly struct Assertable : IAssertable {
        private readonly Exception _excuse;
        public           Exception Excuse => _excuse ?? throw new InvalidOperationException($"Could not retrieve the {nameof(Excuse)} from the {this.GetType().Name} because {nameof(Failed)} = {Failed}!");
        public           bool      Failed => _excuse != null;

        public string       Nickname { get; }
        public Func<string> Message  { get; }

        public Assertable(Action assertion, [CanBeNull] Func<string> message) {
            Nickname = GetNickname(assertion.Method);
            Message  = message;

            try {
                assertion.Invoke();
                _excuse = default;
            }
            catch (SuccessException) {
                _excuse = default;
            }
            catch (Exception e) {
                _excuse = e;
            }
        }

        private static string GetNickname(MethodInfo methodInfo) {
            return methodInfo.Prettify(AssertableExtensions.AssertablePrettificationSettings);
        }

        public override string ToString() {
            return this.FormatAssertable().JoinLines() ?? base.ToString();
        }
    }
}