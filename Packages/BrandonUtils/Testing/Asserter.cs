using System;

using JetBrains.Annotations;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public class Asserter<T> : MultipleAsserter<Asserter<T>, T> {
        protected override Action<string>                     ActionOnFailure          => Assert.Fail;
        protected override Action<T, IResolveConstraint>      ConstraintResolver       => Assert.That;
        protected override Action<object, IResolveConstraint> ObjectConstraintResolver => Assert.That;

        public Asserter() { }
        public Asserter(T actual) : base(actual) { }
    }

    [PublicAPI]
    public static class Asserter {
        [System.Diagnostics.Contracts.Pure]
        public static Asserter<T> Against<T>(T actual) {
            return new Asserter<T>(actual);
        }

        [System.Diagnostics.Contracts.Pure]
        public static Asserter<object> WithHeading(string heading) => new Asserter<object>().WithHeading(heading);
    }
}