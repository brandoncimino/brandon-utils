using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using BrandonUtils.Standalone.Optional;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Chronic {
    [SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
    public static class MethodTimer {
        #region Single Execution

        #region Action (0 args)

        [NotNull] public static ExecutionTime MeasureExecution([NotNull, InstantHandle] Action actionBeingTimed) => MeasureExecution(default, actionBeingTimed);

        [NotNull] public static ExecutionTime MeasureExecution(string? nickname, [NotNull, InstantHandle] Action actionBeingTimed) => MeasureExecution(nickname, actionBeingTimed, new Stopwatch());

        [NotNull]
        private static ExecutionTime MeasureExecution(string? nickname, [NotNull] Action actionBeingTimed, [NotNull] Stopwatch stopwatch) {
            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();
            var result = actionBeingTimed.Try();
            stopwatch.Stop();
            return new ExecutionTime(nickname, result, stopwatch.Elapsed);
        }

        #endregion

        #region Action<T> (1 arg)

        [NotNull]
        public static ExecutionTime MeasureExecution<T>(
            string? nickname,
            [CanBeNull] T       input,
            [NotNull, InstantHandle]
            Action<T> actionBeingTimed
        ) {
            return MeasureExecution(nickname ?? actionBeingTimed.Method.Name, () => actionBeingTimed.Invoke(input));
        }

        [NotNull]
        public static ExecutionTime MeasureExecution<T>(
            [CanBeNull] T input,
            [NotNull, InstantHandle]
            Action<T> actionBeingTimed
        ) {
            return MeasureExecution(default, input, actionBeingTimed);
        }

        #endregion

        #endregion

        #region Multiple Executions

        #region 0 args

        #region Action (0 args)

        [NotNull] public static AggregateExecutionTime MeasureExecution([NotNull] Action action, [NonNegativeValue] int iterations) => MeasureExecution((default, action), iterations);

        [NotNull]
        public static AggregateExecutionTime MeasureExecution((string nickname, Action action) actionBeingTimed, [NonNegativeValue] int iterations) {
            if (actionBeingTimed.action == null) {
                throw new ArgumentNullException(nameof(actionBeingTimed));
            }

            if (iterations <= 0) {
                throw new ArgumentOutOfRangeException(nameof(iterations), iterations, "Must be > 0");
            }

            actionBeingTimed.nickname = actionBeingTimed.nickname.IfBlank(actionBeingTimed.action.Method.Name);

            var results = new ExecutionTime[iterations];
            for (int i = 0; i < iterations; i++) {
                results[i] = MeasureExecution(actionBeingTimed.nickname, actionBeingTimed.action);
            }

            return new AggregateExecutionTime(actionBeingTimed.nickname, results);
        }

        #endregion

        #region Func<T> (0 args)

        [NotNull]
        public static AggregateExecutionTime MeasureExecution<T>(
            (string nickname, Func<T> func) functionBeingTimed,
            [NonNegativeValue]
            int iterations
        ) {
            functionBeingTimed.nickname ??= functionBeingTimed.func.Method.Name;
            return MeasureExecution(
                (
                    functionBeingTimed.nickname,
                    new Action(() => functionBeingTimed.func.Invoke())
                ),
                iterations
            );
        }

        #endregion

        #endregion

        #region 1 arg

        #region Action<T> (1 arg)

        [NotNull]
        public static AggregateExecutionTime MeasureExecution<T>(
            T                                   input,
            (string nickname, Action<T> action) actionBeingTimed,
            [NonNegativeValue]
            int iterations
        ) {
            actionBeingTimed.nickname ??= actionBeingTimed.action.Method.Name;
            return MeasureExecution((actionBeingTimed.nickname, () => actionBeingTimed.action(input)), iterations);
        }

        [NotNull]
        public static AggregateExecutionTime MeasureExecution<T>(
            [CanBeNull] T         input,
            [NotNull]   Action<T> actionBeingTimed,
            [NonNegativeValue]
            int iterations
        ) {
            return MeasureExecution(input, (default, actionBeingTimed), iterations);
        }

        #endregion

        #region Func<TIn, TOut> (1 arg)

        public static AggregateExecutionComparison MeasureExecution<TIn, TOut>(
            TIn                                      input,
            (string nickname, Func<TIn, TOut> func)  first,
            (string nickname, Func<TIn, TOut> first) second,
            [NonNegativeValue]
            int iterations
        ) {
            throw new NotImplementedException();
        }

        [NotNull]
        public static AggregateExecutionComparison MeasureExecution<TIn, TOut>(
            TIn input,
            [NotNull, InstantHandle]
            Func<TIn, TOut> first,
            [NotNull, InstantHandle]
            Func<TIn, TOut> second,
            [NonNegativeValue]
            int iterations
        ) {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Action<T1,T2> (2 args)

        [NotNull]
        public static AggregateExecutionTime MeasureExecution<T1, T2>(
            (T1 arg1, T2 arg2)                       input,
            (string nickname, Action<T1, T2> action) actionBeingTimed,
            [NonNegativeValue]
            int iterations
        ) {
            actionBeingTimed.nickname ??= actionBeingTimed.action.Method.Name;
            return MeasureExecution((actionBeingTimed.nickname, () => actionBeingTimed.action.Invoke(input)), iterations);
        }

        [NotNull]
        public static AggregateExecutionTime MeasureExecution<T1, T2>(
            (T1 arg1, T2 arg2) input,
            [NotNull, InstantHandle]
            Action<T1, T2> actionBeingTimed,
            [NonNegativeValue]
            int iterations
        ) {
            return MeasureExecution(input, (default, actionBeingTimed), iterations);
        }

        #endregion

        #region Func<T1, T2, TOut> (2 args)

        //TODO

        #endregion

        #endregion

        #region Comparisons

        #region Action (0 args)

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions(
            (string nickname, Action action) first,
            (string nickname, Action action) second,
            int                              iterations
        ) {
            if (first.action == null) {
                throw new ArgumentNullException($"{nameof(first)}.{nameof(first.action)}");
            }

            if (second.action == null) {
                throw new ArgumentNullException($"{nameof(second)}.{nameof(second.action)}");
            }

            var firstTimes  = MeasureExecution(first,  iterations);
            var secondTimes = MeasureExecution(second, iterations);

            var comparison = new AggregateExecutionComparison(firstTimes, secondTimes);
            Console.WriteLine(comparison);
            return comparison;
        }

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions([NotNull] Action firstAction, [NotNull] Action secondAction, int iterations) {
            return CompareExecutions(
                (default, firstAction),
                (default, secondAction),
                iterations
            );
        }

        #endregion

        #region Action<T> (1 arg)

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions<T>(
            [CanBeNull] T                       input,
            (string nickname, Action<T> action) first,
            (string nickname, Action<T> action) second,
            [NonNegativeValue]
            int iterations
        ) {
            return CompareExecutions(
                (first.nickname, () => first.action(input)),
                (second.nickname, () => second.action(input)),
                iterations
            );
        }

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions<T>(
            [CanBeNull] T         input,
            [NotNull]   Action<T> first,
            [NotNull]   Action<T> second,
            [NonNegativeValue]
            int iterations
        ) {
            return CompareExecutions(
                input,
                (default, first),
                (default, second),
                iterations
            );
        }

        #endregion

        #region Func<TIn, TOut> (1 arg)

        //TODO

        #endregion"

        #region Action<T1,T2> (2 args)

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions<T1, T2>(
            (T1 arg1, T2 arg2)                       input,
            (string nickname, Action<T1, T2> action) first,
            (string nickname, Action<T1, T2> action) second,
            [NonNegativeValue]
            int iterations
        ) {
            return CompareExecutions(
                (first.nickname, () => first.action.Invoke(input)),
                (second.nickname, () => second.action.Invoke(input)),
                iterations
            );
        }

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions<T1, T2>(
            (T1 arg1, T2 arg2)       input,
            [NotNull] Action<T1, T2> first,
            [NotNull] Action<T1, T2> second,
            [NonNegativeValue]
            int iterations
        ) {
            return CompareExecutions(
                input,
                (default, first),
                (default, second),
                iterations
            );
        }

        #endregion

        #region Func<T1, T2, TOut> (2 args)

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions<T1, T2, TOut>(
            (T1 arg1, T2 arg2)                         input,
            (string nickname, Func<T1, T2, TOut> func) first,
            (string nickname, Func<T1, T2, TOut> func) second,
            [NonNegativeValue]
            int iterations
        ) {
            first.nickname  ??= first.func.Method.Name;
            second.nickname ??= second.func.Method.Name;

            return CompareExecutions(
                (first.nickname, () => _ = first.func.Invoke(input)),
                (second.nickname, () => _ = second.func.Invoke(input)),
                iterations
            );
        }

        [NotNull]
        public static AggregateExecutionComparison CompareExecutions<T1, T2, TOut>(
            (T1 arg1, T2 arg2) input,
            [NotNull, InstantHandle]
            Func<T1, T2, TOut> first,
            [NotNull, InstantHandle]
            Func<T1, T2, TOut> second,
            [NonNegativeValue]
            int iterations
        ) {
            return CompareExecutions(
                input,
                (default, first),
                (default, second),
                iterations
            );
        }

        #endregion

        #endregion
    }
}