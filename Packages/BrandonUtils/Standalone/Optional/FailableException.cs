using System;

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

        internal static InvalidOperationException DidNotFailException<TValue, TExcuse>(IFailableFunc<TValue, TExcuse> failableFunc) where TExcuse : Exception {
            return new InvalidOperationException($"{DidNotFailMessage(failableFunc)} (Actual {nameof(failableFunc.Value)}: {failableFunc.Value})");
        }

        internal static InvalidOperationException FailedException<TValue, TExcuse>(IFailableFunc<TValue, TExcuse> failableFunc) where TExcuse : Exception {
            return new InvalidOperationException($"Unable to retrieve the {typeof(TValue).Name} {nameof(failableFunc.Value)} from the {failableFunc.GetType().Name} because it failed!", failableFunc.Excuse);
        }
    }
}