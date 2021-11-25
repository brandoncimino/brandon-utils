using System;
using System.Diagnostics.Contracts;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace BrandonUtils.Testing {
    public class Asserter<T> : MultipleAsserter<Asserter<T>, T> {
        protected override Action<string>                                                        ActionOnFailure          => Assert.Fail;
        protected override Action<ActualValueDelegate<object>, IResolveConstraint, Func<string>> TrueResolver             => Assert.That;
        protected override Action<ActualValueDelegate<T>, IResolveConstraint, Func<string>>      TrueTypeResolver         => Assert.That;
        protected override Action<TestDelegate, IResolveConstraint, Func<string>>                ActionConstraintResolver => Assert.That;

        public Asserter() { }
        public Asserter(T                      actual) : base(actual) { }
        public Asserter(ActualValueDelegate<T> actualValueDelegate) : base(actualValueDelegate) { }
    }

    [PublicAPI]
    public static class Asserter {
        [Pure]
        public static Asserter<T> Against<T>(T actual) {
            return new Asserter<T>(actual);
        }

        [Pure]
        public static Asserter<T> Against<T>(ActualValueDelegate<T> actualValueDelegate) {
            return new Asserter<T>(actualValueDelegate);
        }

        [Pure] public static Asserter<object> WithHeading(string heading) => new Asserter<object>().WithHeading(heading);
    }
}