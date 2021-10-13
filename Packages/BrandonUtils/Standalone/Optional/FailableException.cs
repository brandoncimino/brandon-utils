using System;

using BrandonUtils.Standalone.Strings.Prettifiers;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /// <summary>
    /// Static methods for building <see cref="Exception"/>s used by <see cref="IFailable{TExcuse}"/> implementations.
    /// </summary>
    internal static class FailableException {
        private static string DidNotFailMessage<TExcuse>(IFailable<TExcuse> failable) where TExcuse : Exception {
            return $"Unable to retrieve the {nameof(failable.Excuse)} from the {failable.GetType().Name} because {nameof(failable.Failed)} == {failable.Failed}!";
        }

        internal static InvalidOperationException DidNotFailException<TExcuse>(IFailable<TExcuse> failable) where TExcuse : Exception {
            return new InvalidOperationException(DidNotFailMessage(failable));
        }

        internal static InvalidOperationException DidNotFailException<TValue, TExcuse>(IFailableFunc<TValue, TExcuse> failableFunc, Optional<TValue> actualValue) where TExcuse : Exception {
            var msg = DidNotFailMessage(failableFunc);

            if (actualValue.HasValue) {
                msg += $"({nameof(actualValue)}: {actualValue})";
            }

            throw new InvalidOperationException(msg);
        }

        [NotNull]
        internal static InvalidOperationException FailedException<TValue, TExcuse>([NotNull] IFailableFunc<TValue, TExcuse> failableFunc, Optional<TExcuse> actualExcuse) where TExcuse : Exception {
            return new InvalidOperationException($"Unable to retrieve the {typeof(TValue).PrettifyType()} {nameof(failableFunc.Value)} from the {failableFunc.GetType().Name} because it failed!", actualExcuse.OrElse(default));
        }
    }
}