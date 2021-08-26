using System;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BrandonUtils.Testing {
    public class Ignorer<T> : MultipleAsserter<Ignorer<T>, T> {
        protected override Action<string>                ActionOnFailure    => Assert.Ignore;
        protected override Action<T, IResolveConstraint> ConstraintResolver => TestConstraintForIgnore;

        private static void TestConstraintForIgnore(T actual, IResolveConstraint constraint) {
            var appliedConstraint = constraint.Resolve().ApplyTo(actual);
            if (appliedConstraint.IsSuccess == false) {
                Assert.Ignore(appliedConstraint.Description);
            }
        }

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