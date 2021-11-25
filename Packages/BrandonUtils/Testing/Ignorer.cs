using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public class Ignorer<T> : MultipleAsserter<Ignorer<T>, T> {
        protected override Action<string>                                                        ActionOnFailure          => Assert.Ignore;
        protected override Action<ActualValueDelegate<object>, IResolveConstraint, Func<string>> TrueResolver             => Ignore.Unless;
        protected override Action<ActualValueDelegate<T>, IResolveConstraint, Func<string>>      TrueTypeResolver         => Ignore.Unless;
        protected override Action<TestDelegate, IResolveConstraint, Func<string>>                ActionConstraintResolver => Ignore.Unless;

        public Ignorer() { }

        public Ignorer(T actual) : base(actual) { }
    }

    public static class Ignorer {
        public static Ignorer<T> Against<T>(T actual) {
            return new Ignorer<T>(actual);
        }

        public static Ignorer<object> WithHeading(string heading) => new Ignorer<object>().WithHeading(heading);
    }
}