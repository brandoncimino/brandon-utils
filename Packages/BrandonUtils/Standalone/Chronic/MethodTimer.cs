using System;
using System.Diagnostics;

using BrandonUtils.Standalone.Optional;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Chronic {
    public static class MethodTimer {
        [NotNull]
        public static ExecutionTime MeasureExecution([NotNull] Action actionBeingTimed) {
            return MeasureExecution(actionBeingTimed, new Stopwatch());
        }

        [NotNull]
        private static ExecutionTime MeasureExecution([NotNull] Action actionBeingTimed, [NotNull] Stopwatch stopwatch) {
            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();
            var result = actionBeingTimed.Try();
            stopwatch.Stop();
            return new ExecutionTime(result, stopwatch.Elapsed);
        }

        [NotNull]
        public static AggregateExecutionTime MeasureExecution([NotNull] Action action, [NonNegativeValue] int iterations) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }

            if (iterations <= 0) {
                throw new ArgumentOutOfRangeException(nameof(iterations), iterations, "Must be > 0");
            }

            var results = new ExecutionTime[iterations];
            for (int i = 0; i < iterations; i++) {
                results[i] = MeasureExecution(action);
            }

            return new AggregateExecutionTime(results);
        }

        public static AggregateExecutionComparison CompareExecutions([NotNull] Action firstAction, [NotNull] Action secondAction, int iterations) {
            if (firstAction == null) {
                throw new ArgumentNullException(nameof(firstAction));
            }

            if (secondAction == null) {
                throw new ArgumentNullException(nameof(secondAction));
            }

            var firstTimes  = MeasureExecution(firstAction,  iterations);
            var secondTimes = MeasureExecution(secondAction, iterations);

            var comparison = new AggregateExecutionComparison(firstTimes, secondTimes);
            Console.WriteLine(comparison);
            return comparison;
        }
    }
}