using System;

using JetBrains.Annotations;

namespace BrandonUtils.Standalone.Optional {
    /**
     * <inheritdoc cref="IFailable"/>
     */
    public readonly struct Failable : IFailable {
        private const string SuccessIcon = "✅";
        private const string FailIcon    = "❌";

        private readonly Exception _excuse;

        public Exception Excuse => _excuse ?? throw FailableException.DidNotFailException(this);

        public bool Failed => _excuse != null;

        public Failable(Action failableAction) {
            try {
                failableAction.Invoke();
                _excuse = default;
            }
            catch (Exception e) {
                _excuse = e;
            }
        }

        [NotNull]
        public override string ToString() {
            return $"{(Failed ? $"{FailIcon} [{Excuse}]" : SuccessIcon)}";
        }
    }
}