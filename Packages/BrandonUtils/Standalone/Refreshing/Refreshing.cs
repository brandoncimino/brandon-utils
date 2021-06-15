using System;

using BrandonUtils.Standalone.Optional;

namespace BrandonUtils.Standalone.Refreshing {
    /// <summary>
    /// Represents a value that:
    /// <ul>
    /// <li>Is expensive to generate</li>
    /// <li>Can become <see cref="Refreshing.Freshness.Stale"/>, at which point it will need to be <see cref="Refresh"/>ed</li>
    /// <li>Has a <see cref="StalenessPredicate"/> that is cheap and simple</li>
    /// <li>Has a <see cref="StalenessPredicate"/> that callers don't need to worry about</li>
    /// </ul>
    /// </summary>
    /// <remarks>
    /// A cute alternative to the name "Refreshing" would be "Slacking" or "Slacker", because it:
    /// <ul>
    /// <li>Is <see cref="System.Lazy{T}"/> as often as possible</li>
    /// <li>Works only when you ask it to</li>
    /// </ul>
    /// <b>NOTE:</b> The <see cref="Freshness"/> does <b>not</b> imply anything about the <b>accuracy</b> of the <see cref="Value"/>.
    /// </remarks>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TStaleness"></typeparam>
    public class Refreshing<TValue, TStaleness> : IEquatable<TValue>
        where TValue : IEquatable<TValue> {
        /// <summary>
        /// The latest "cached" <see cref="TValue"/>.
        /// </summary>
        private TValue _value;

        /// <summary>
        /// The "C# style" of retrieving the value from the <see cref="Refreshing{TValue,TStaleness}"/>
        /// (mimicking <see cref="System.Lazy{T}"/>.<see cref="Lazy{T}.Value"/>).
        /// </summary>
        public TValue Value {
            get {
                if (Freshness != Freshness.Fresh) {
                    Refresh();
                }

                return _value;
            }
        }

        /// <summary>
        /// The number of times that <see cref="RefreshCount"/> has been called
        /// </summary>
        public int RefreshCount { get; private set; }

        /// <summary>
        /// The current <see cref="Freshness"/> of the <see cref="_value"/>.
        /// </summary>
        public Freshness Freshness {
            get {
                if (PreviousStalenessBasis.HasValue) {
                    return IsStale ? Freshness.Stale : Freshness.Fresh;
                }
                else {
                    return Freshness.Pristine;
                }
            }
        }

        private bool IsStale {
            get {
                if (!PreviousStalenessBasis.HasValue) {
                    return false;
                }

                var currentStalenessBasis = StalenessBasisSupplier.Invoke();
                return StalenessPredicate.Invoke(PreviousStalenessBasis.Value, currentStalenessBasis);
            }
        }

        /// <summary>
        /// The expensive function that produces <see cref="Refreshing.Freshness.Fresh"/> <see cref="Value"/>s.
        /// </summary>
        public CountedFunc<TValue> ValueSupplier { get; }

        /// <summary>
        /// The function that determines if a value is <see cref="Refreshing.Freshness.Stale"/>.
        /// </summary>
        public Func<TStaleness, TStaleness, bool> StalenessPredicate { get; }

        /// <summary>
        /// prisdabn;laskjdf
        /// </summary>
        public Func<TStaleness> StalenessBasisSupplier { get; }
        public Optional<TStaleness> PreviousStalenessBasis;

        public Refreshing(Func<TValue> valueSupplier, Func<TStaleness> stalenessBasisSupplier, Func<TStaleness, TStaleness, bool> stalenessPredicate) {
            this.ValueSupplier          = valueSupplier;
            this.StalenessBasisSupplier = stalenessBasisSupplier;
            this.StalenessPredicate     = stalenessPredicate;
        }

        /// <summary>
        /// Forces an invocation of <see cref="ValueSupplier"/> and stores the result in <see cref="_value"/>, regardless of the current <see cref="Freshness"/>.
        /// </summary>
        /// <remarks>
        /// This method should rarely be called externally, as it bypasses the <see cref="StalenessPredicate"/> and <b>always</b> invokes the <see cref="ValueSupplier"/>.
        /// Most of the time, you should call <see cref="Value"/> instead.
        /// </remarks>
        /// <returns>the result of <see cref="ValueSupplier"/></returns>
        public TValue Refresh() {
            _value                 = ValueSupplier.Invoke();
            PreviousStalenessBasis = StalenessBasisSupplier.Invoke();
            return _value;
        }

        public bool Equals(TValue other) {
            return Value.Equals(other);
        }
    }
}