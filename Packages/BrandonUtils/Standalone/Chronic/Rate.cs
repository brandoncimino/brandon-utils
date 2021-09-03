using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Chronic {
    /// <summary>
    /// Represents an arbitrary quantity over time.
    /// </summary>
    /// <remarks>
    /// The primary purpose of this is to avoid the ambiguity between manipulating a value as "X (per second)" vs. "X seconds per each".
    /// </remarks>
    [PublicAPI]
    public struct Rate {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The amount per 1 <see cref="TimeUnit.Seconds"/>.
        /// </summary>
        public double Hertz { get; set; }

        /// <summary>
        /// The <see cref="TimeSpan"/> of 1 "event" (e.g. 2 <see cref="Hertz"/> = 0.5 <see cref="TimeUnit.Seconds"/>)
        /// </summary>
        public TimeSpan Interval {
            get => HertzToInterval(Hertz);
            set => Hertz = IntervalToHertz(value);
        }

        public double Per(TimeSpan timeSpan) {
            return timeSpan.Divide(Interval);
        }

        public double Per(TimeUnit timeUnit) {
            return Per(timeUnit.SpanOf());
        }

        private static double IntervalToHertz(TimeSpan interval) {
            return interval == TimeSpan.Zero ? double.PositiveInfinity : OneSecond.Divide(interval);
        }

        private static TimeSpan HertzToInterval(double hertz) {
            return hertz == 0 ? default : OneSecond.Divide(hertz);
        }
    }
}