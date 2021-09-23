using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public class Ignorer<T> : MultipleAsserter<Ignorer<T>, T> {
        protected override Action<string>                     ActionOnFailure          => Assert.Ignore;
        protected override Action<T, IResolveConstraint>      ConstraintResolver       => Ignore.Unless;
        protected override Action<object, IResolveConstraint> ObjectConstraintResolver => Ignore.Unless;

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