using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Chronic {
    /// <summary>
    /// Wraps a <see cref="List{T}"/> of <see cref="ExecutionTime"/>s and provides some convenient Linq methods and a flexible <see cref="CompareTo"/>.
    /// </summary>
    /// <remarks>
    /// From Brandon on 8/15/2021: What <i>is</i> this...?
    /// <br/>
    /// From Brandon on 10/15/2021: I think I just started making this again.
    /// </remarks>
    [PublicAPI]
    public class AggregateExecutionTime : IComparable<AggregateExecutionTime> {
        [NotNull, ItemNotNull]
        public ExecutionTime[] Executions { get; }

        public int Iterations => Executions.Length;

        public long MinTicks => Executions.Min(it => it.Duration.Ticks);

        public TimeSpan Min => TimeSpan.FromTicks(MinTicks);

        public long MaxTicks => Executions.Max(it => it.Duration.Ticks);

        public TimeSpan Max => TimeSpan.FromTicks(MaxTicks);

        public double AverageTicks => Executions.Average(it => it.Duration.Ticks);

        public TimeSpan Average => TimeSpan.FromTicks((long)AverageTicks);

        public long TotalTicks => Executions.Sum(it => it.Duration.Ticks);

        public TimeSpan Total => TimeSpan.FromTicks(TotalTicks);

        public double SuccessRate => (double)Executions.Count(it => it.Execution.Failed == false) / Executions.Length;

        internal AggregateExecutionTime([NotNull, ItemNotNull] IEnumerable<ExecutionTime> executions) {
            Executions = executions.ToArray();
        }

        /// <summary>
        /// Compares <b><i>this</i></b> to <paramref name="other"/> using <see cref="CompareTo"/> calls against multiple properties:
        /// <li><see cref="Min"/></li>
        /// <li><see cref="Max"/></li>
        /// <li><see cref="Average"/></li>
        /// Returns -1 or 1 if <b>any</b> properties return that value and <b>none</b> return the other;
        /// otherwise, returns 0.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo([NotNull] AggregateExecutionTime other) {
            if (other == null) {
                throw new ArgumentNullException(nameof(other));
            }

            var minCompare = Min.CompareTo(other.Min);
            var maxCompare = Max.CompareTo(other.Max);
            var avgCompare = Average.CompareTo(other.Average);

            var compares = new int[] { minCompare, maxCompare, avgCompare };
            if (compares.Any(it => it > 0) && compares.All(it => it >= 0)) {
                return 1;
            }
            else if (compares.Any(it => it < 0) && compares.All(it => it <= 0)) {
                return -1;
            }
            else {
                return 0;
            }
        }

        [NotNull]
        public override string ToString() {
            if (Executions == null) {
                throw new ArgumentNullException(nameof(Executions));
            }

            if (Executions.Contains(null)) {
                throw new ArgumentNullException($"{nameof(Executions)} contained null! {Executions.Prettify()}");
            }

            return new Dictionary<object, object>() {
                [nameof(Total)]   = Total,
                [nameof(Average)] = Average,
                [nameof(Min)]     = Min,
                [nameof(Max)]     = Max,
            }.Prettify();
        }
    }

    public class AggregateExecutionComparison {
        public AggregateExecutionTime FirstTimes;
        public AggregateExecutionTime SecondTimes;

        public AggregateExecutionComparison(AggregateExecutionTime firstTimes, AggregateExecutionTime secondTimes) {
            FirstTimes  = firstTimes;
            SecondTimes = secondTimes;
        }

        public readonly struct TimeComparison {
            public readonly TimeSpan First;
            public readonly TimeSpan Second;
            public          TimeSpan Difference => First - Second;
            public          double   Ratio      => First.Divide(Second);

            public TimeComparison(TimeSpan first, TimeSpan second) {
                First  = first;
                Second = second;
            }

            [NotNull]
            public override string ToString() {
                return new Dictionary<object, object>() {
                        [nameof(First)]      = First,
                        [nameof(Second)]     = Second,
                        [nameof(Difference)] = Difference,
                        [nameof(Ratio)]      = Ratio
                    }.AsEnumerable()
                     .JoinString(", ");
                //.Prettify(LineStyle.Single);
            }
        }

        private TimeComparison Comparing([NotNull] Func<AggregateExecutionTime, TimeSpan> extractor) {
            return new TimeComparison(extractor.Invoke(FirstTimes), extractor.Invoke(SecondTimes));
        }

        public TimeComparison Total   => Comparing(it => it.Total);
        public TimeComparison Average => Comparing(it => it.Average);

        [NotNull]
        public override string ToString() {
            return new Dictionary<object, object>() {
                [nameof(Total)]   = Total,
                [nameof(Average)] = Average
            }.Prettify();
        }
    }
}