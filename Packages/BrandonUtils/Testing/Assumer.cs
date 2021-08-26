using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public class Assumer<T> : MultipleAsserter<Assumer<T>, T> {
        protected override Action<string>                ActionOnFailure    => Assert.Inconclusive;
        protected override Action<T, IResolveConstraint> ConstraintResolver => Assume.That;

        public Assumer() { }
        public Assumer(T actual) : base(actual) { }
    }

    public static class Assumer {
        public static Assumer<T> Against<T>(T actual) {
            return new Assumer<T>(actual);
        }

        public static Assumer<object> WithHeading(string heading) => new Assumer<object>().WithHeading(heading);
    }
}